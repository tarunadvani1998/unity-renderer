using System;
using DCL.Controllers;
using DCL.Helpers;
using DCL.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCL.Components
{
    public class PBRMaterial : BaseDisposable
    {
        [System.Serializable]
        public class Model : BaseModel
        {
            [Range(0f, 1f)]
            public float alphaTest = 0.5f;

            public Color albedoColor = Color.white;
            public string albedoTexture;
            public float metallic = 0.5f;
            public float roughness = 0.5f;
            public float microSurface = 1f; // Glossiness
            public float specularIntensity = 1f;

            public string alphaTexture;
            public string emissiveTexture;
            public Color emissiveColor = Color.black;
            public float emissiveIntensity = 2f;
            public Color reflectivityColor = Color.white;
            public float directIntensity = 1f;
            public string bumpTexture;
            public bool castShadows = true;

            [Range(0, 4)]
            public int transparencyMode = 4; // 0: OPAQUE; 1: ALPHATEST; 2: ALPHBLEND; 3: ALPHATESTANDBLEND; 4: AUTO (Engine decide)

            public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((Model) obj);
            }
            
            protected bool Equals(Model other)
            {
                return alphaTest.Equals(other.alphaTest) && albedoColor.Equals(other.albedoColor) && albedoTexture == other.albedoTexture && metallic.Equals(other.metallic) && roughness.Equals(other.roughness) && microSurface.Equals(other.microSurface) && specularIntensity.Equals(other.specularIntensity) && alphaTexture == other.alphaTexture && emissiveTexture == other.emissiveTexture && emissiveColor.Equals(other.emissiveColor) && emissiveIntensity.Equals(other.emissiveIntensity) && reflectivityColor.Equals(other.reflectivityColor) && directIntensity.Equals(other.directIntensity) && bumpTexture == other.bumpTexture && castShadows == other.castShadows && transparencyMode == other.transparencyMode;
            }
            
            public override int GetHashCode() {
                unchecked
                {
                    int hashCode = alphaTest.GetHashCode();
                    hashCode = (hashCode * 397) ^ albedoColor.GetHashCode();
                    hashCode = (hashCode * 397) ^ (albedoTexture != null ? albedoTexture.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ metallic.GetHashCode();
                    hashCode = (hashCode * 397) ^ roughness.GetHashCode();
                    hashCode = (hashCode * 397) ^ microSurface.GetHashCode();
                    hashCode = (hashCode * 397) ^ specularIntensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ (alphaTexture != null ? alphaTexture.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (emissiveTexture != null ? emissiveTexture.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ emissiveColor.GetHashCode();
                    hashCode = (hashCode * 397) ^ emissiveIntensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ reflectivityColor.GetHashCode();
                    hashCode = (hashCode * 397) ^ directIntensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ (bumpTexture != null ? bumpTexture.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ castShadows.GetHashCode();
                    hashCode = (hashCode * 397) ^ transparencyMode;
                    return hashCode;
                }
            }
        }

        enum TransparencyMode
        {
            OPAQUE,
            ALPHA_TEST,
            ALPHA_BLEND,
            ALPHA_TEST_AND_BLEND,
            AUTO
        }

        public Material material { get; set; }
        private string currentMaterialResourcesFilename;

        const string MATERIAL_RESOURCES_PATH = "Materials/";
        const string PBR_MATERIAL_NAME = "ShapeMaterial";

        DCLTexture albedoDCLTexture = null;
        DCLTexture alphaDCLTexture = null;
        DCLTexture emissiveDCLTexture = null;
        DCLTexture bumpDCLTexture = null;

        private Model oldModel;

        private List<Coroutine> textureFetchCoroutines = new List<Coroutine>();

        public PBRMaterial()
        {
            model = new Model();

            OnAttach += OnMaterialAttached;
            OnDetach += OnMaterialDetached;
        }

        new public Model GetModel() { return (Model) model; }

        public override int GetClassId() { return (int) CLASS_ID.PBR_MATERIAL; }

        public override void AttachTo(IDCLEntity entity, System.Type overridenAttachedType = null)
        {
            if (attachedEntities.Contains(entity))
            {
                return;
            }

            entity.RemoveSharedComponent(typeof(BasicMaterial));
            base.AttachTo(entity);
        }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            Model model = (Model) newModel;

            Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().ForgetMaterial(oldModel);
            oldModel = model;
            AsignMaterial(model);

            // Note: Ugly wait to insert Unitask in the components
            yield return new WaitUntil(() => material != null);
            
            foreach (IDCLEntity entity in attachedEntities)
                InitMaterial(entity);
        }

        private async void AsignMaterial(Model model)
        {
            var wrapper = await Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().GetMaterial(model);
            material = wrapper.Get();
        }

        void OnMaterialAttached(IDCLEntity entity)
        {
            entity.OnShapeUpdated -= OnShapeUpdated;
            entity.OnShapeUpdated += OnShapeUpdated;

            if (entity.meshRootGameObject != null)
            {
                var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

                if (meshRenderer != null)
                    InitMaterial(entity);
            }
        }

        void InitMaterial(IDCLEntity entity)
        {
            var meshGameObject = entity.meshRootGameObject;

            if (meshGameObject == null)
                return;

            var meshRenderer = meshGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                return;

            Model model = (Model) this.model;

            meshRenderer.shadowCastingMode = model.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

            if (meshRenderer.sharedMaterial == material)
                return;

            MaterialTransitionController
                matTransition = meshGameObject.GetComponent<MaterialTransitionController>();

            if (matTransition != null && matTransition.canSwitchMaterial)
            {
                matTransition.finalMaterials = new Material[] { material };
                matTransition.PopulateTargetRendererWithMaterial(matTransition.finalMaterials);
            }

            Material oldMaterial = meshRenderer.sharedMaterial;
            meshRenderer.sharedMaterial = material;
            SRPBatchingHelper.OptimizeMaterial(material);

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, oldMaterial);
            DataStore.i.sceneWorldObjects.AddMaterial(scene.sceneData.id, entity.entityId, material);
        }

        private void OnShapeUpdated(IDCLEntity entity)
        {
            if (entity != null)
                InitMaterial(entity);
        }

        private void OnMaterialDetached(IDCLEntity entity)
        {
            if (entity.meshRootGameObject == null)
                return;

            entity.OnShapeUpdated -= OnShapeUpdated;

            var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer && meshRenderer.sharedMaterial == material)
                meshRenderer.sharedMaterial = null;

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, material);
        }

        public override void Dispose()
        {
            albedoDCLTexture?.DetachFrom(this);
            alphaDCLTexture?.DetachFrom(this);
            emissiveDCLTexture?.DetachFrom(this);
            bumpDCLTexture?.DetachFrom(this);

            for (int i = 0; i < textureFetchCoroutines.Count; i++)
            {
                var coroutine = textureFetchCoroutines[i];

                if ( coroutine != null )
                    CoroutineStarter.Stop(coroutine);
            }
            
            Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().ForgetMaterial(oldModel);

            base.Dispose();
        }
    }
}