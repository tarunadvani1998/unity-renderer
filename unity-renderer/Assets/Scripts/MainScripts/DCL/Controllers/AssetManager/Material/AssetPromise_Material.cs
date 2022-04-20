using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DCL.Helpers;
using UnityEngine;

namespace DCL
{
    public class AssetPromise_Material : AssetPromise<Asset_Material>
    {
        const string MATERIAL_RESOURCES_PATH = "Materials/";
        const string PBR_MATERIAL_NAME = "ShapeMaterial";

        enum TransparencyMode
        {
            OPAQUE,
            ALPHA_TEST,
            ALPHA_BLEND,
            ALPHA_TEST_AND_BLEND,
            AUTO
        }

        private DCLTexture albedoDCLTexture = null;
        private DCLTexture alphaDCLTexture = null;
        private DCLTexture emissiveDCLTexture = null;
        private DCLTexture bumpDCLTexture = null;

        private List<Coroutine> textureFetchCoroutines = new List<Coroutine>();

        private AssetPromise_DCLTexture texturePromiseKeeper;

        private PBRMaterialModel model;
        public AssetPromise_Material(PBRMaterialModel model)
        {
            this.model = model;
        }
        
        protected override void OnAfterLoadOrReuse() { throw new NotImplementedException(); }

        protected override void OnBeforeLoadOrReuse() { throw new NotImplementedException(); }

        protected override void OnCancelLoading() { throw new NotImplementedException(); }

        protected override void OnLoad(Action OnSuccess, Action<Exception> OnFail) { throw new NotImplementedException(); }

        public override object GetId() { return model.GetHashCode(); }
        
        private Material LoadMaterial(PBRMaterialModel model, string resourcesFilename)
    {
        Material material = new Material(Utils.EnsureResourcesMaterial(MATERIAL_RESOURCES_PATH + resourcesFilename));
#if UNITY_EDITOR
        material.name = "PBRMaterial_" + model.GetHashCode();
#endif
        return material;
    }

    private async UniTask<Material> SetupMaterial(PBRMaterialModel model)
    {
        Material material = LoadMaterial(model, PBR_MATERIAL_NAME);

        material.SetColor(ShaderUtils.BaseColor, model.albedoColor);

        if (model.emissiveColor != Color.clear && model.emissiveColor != Color.black)
        {
            material.EnableKeyword("_EMISSION");
        }

        // METALLIC/SPECULAR CONFIGURATIONS
        material.SetColor(ShaderUtils.EmissionColor, model.emissiveColor * model.emissiveIntensity);
        material.SetColor(ShaderUtils.SpecColor, model.reflectivityColor);

        material.SetFloat(ShaderUtils.Metallic, model.metallic);
        material.SetFloat(ShaderUtils.Smoothness, 1 - model.roughness);
        material.SetFloat(ShaderUtils.EnvironmentReflections, model.microSurface);
        material.SetFloat(ShaderUtils.SpecularHighlights, model.specularIntensity * model.directIntensity);
        

        // FETCH AND LOAD EMISSIVE TEXTURE
        var fetchEmission = FetchTexture(material,ShaderUtils.EmissionMap, model.emissiveTexture, emissiveDCLTexture);

        SetupTransparencyMode(model,material);

        // FETCH AND LOAD TEXTURES
        var fetchBaseMap = FetchTexture(material,ShaderUtils.BaseMap, model.albedoTexture, albedoDCLTexture);
        var fetchAlpha = FetchTexture(material,ShaderUtils.AlphaTexture, model.alphaTexture, alphaDCLTexture);
        var fetchBump = FetchTexture(material,ShaderUtils.BumpMap, model.bumpTexture, bumpDCLTexture);

        textureFetchCoroutines.Add(CoroutineStarter.Start(fetchEmission));
        textureFetchCoroutines.Add(CoroutineStarter.Start(fetchBaseMap));
        textureFetchCoroutines.Add(CoroutineStarter.Start(fetchAlpha));
        textureFetchCoroutines.Add(CoroutineStarter.Start(fetchBump));

        await fetchBaseMap;
        await fetchAlpha;
        await fetchBump;
        await fetchEmission;

        return material;
    }

    IEnumerator FetchTexture(Material material,int materialPropertyId, string textureComponentId, DCLTexture cachedDCLTexture)
    {
        if (!string.IsNullOrEmpty(textureComponentId))
        {
            if (!AreSameTextureComponent(cachedDCLTexture, textureComponentId))
            {
                yield return DCLTexture.FetchTextureComponent(textureComponentId,
                    (fetchedDCLTexture) =>
                    {
                        if (material == null)
                            return;
                
                        material.SetTexture(materialPropertyId, fetchedDCLTexture.texture);
                        SwitchTextureComponent(cachedDCLTexture, fetchedDCLTexture);
                    });
            }
        }
        else
        {
            material.SetTexture(materialPropertyId, null);
            // cachedDCLTexture?.DetachFrom(this);
        }
        yield return null;
    }
    
    private bool AreSameTextureComponent(DCLTexture dclTexture, string textureId)
    {
        if (dclTexture == null)
            return false;
        return dclTexture.id == textureId;
    }
    
    void SwitchTextureComponent(DCLTexture cachedTexture, DCLTexture newTexture)
    {
        cachedTexture = newTexture;
    }

    private void SetupTransparencyMode(PBRMaterialModel model, Material material)
    {

        // Reset shader keywords
        material.DisableKeyword("_ALPHATEST_ON"); // Cut Out Transparency
        material.DisableKeyword("_ALPHABLEND_ON"); // Fade Transparency
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Transparent

        TransparencyMode transparencyMode = (TransparencyMode) model.transparencyMode;

        if (transparencyMode == TransparencyMode.AUTO)
        {
            if (!string.IsNullOrEmpty(model.alphaTexture) || model.albedoColor.a < 1f) //AlphaBlend
            {
                transparencyMode = TransparencyMode.ALPHA_BLEND;
            }
            else // Opaque
            {
                transparencyMode = TransparencyMode.OPAQUE;
            }
        }

        switch (transparencyMode)
        {
            case TransparencyMode.OPAQUE:
                material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Geometry;
                material.SetFloat(ShaderUtils.AlphaClip, 0);
                break;
            case TransparencyMode.ALPHA_TEST: // ALPHATEST
                material.EnableKeyword("_ALPHATEST_ON");

                material.SetInt(ShaderUtils.SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                material.SetInt(ShaderUtils.DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt(ShaderUtils.ZWrite, 1);
                material.SetFloat(ShaderUtils.AlphaClip, 1);
                material.SetFloat(ShaderUtils.Cutoff, model.alphaTest);
                material.SetInt("_Surface", 0);
                material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case TransparencyMode.ALPHA_BLEND: // ALPHABLEND
                material.EnableKeyword("_ALPHABLEND_ON");

                material.SetInt(ShaderUtils.SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt(ShaderUtils.DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt(ShaderUtils.ZWrite, 0);
                material.SetFloat(ShaderUtils.AlphaClip, 0);
                material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                material.SetInt("_Surface", 1);
                break;
            case TransparencyMode.ALPHA_TEST_AND_BLEND:
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");

                material.SetInt(ShaderUtils.SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                material.SetInt(ShaderUtils.DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt(ShaderUtils.ZWrite, 0);
                material.SetFloat(ShaderUtils.AlphaClip, 1);
                material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                material.SetInt("_Surface", 1);
                break;
        }
    }
    }
}