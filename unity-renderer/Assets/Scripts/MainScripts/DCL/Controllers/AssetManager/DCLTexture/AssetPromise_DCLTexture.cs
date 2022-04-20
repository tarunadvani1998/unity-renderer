using System;
using System.Collections;
using DCL.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace DCL
{
    public class AssetPromise_DCLTexture : AssetPromise<Asset_DCLTexture>
    {

        public FilterMode unitySamplingMode;
        public Texture2D texture { get; protected set; }
        
        private AssetPromise_Texture texturePromise = null;
        
        private DCLTextureModel model;
        private string contenstUrl;

        private Coroutine loadCoroutine;
        
        public AssetPromise_DCLTexture(DCLTextureModel model, string contenstUrl)
        {
            this.model = model;
            this.contenstUrl = contenstUrl;
        }

        protected override void OnAfterLoadOrReuse() { }

        protected override void OnBeforeLoadOrReuse() { }

        protected override object GetLibraryAssetCheckId() { return model;}

        protected override void OnCancelLoading()
        {
            CoroutineStarter.Stop(loadCoroutine);
            
            if (texturePromise != null)
            {
                AssetPromiseKeeper_Texture.i.Forget(texturePromise);
                texturePromise = null;
            }
        }

        protected override void OnLoad(Action OnSuccess, Action<Exception> OnFail)
        {
            loadCoroutine = CoroutineStarter.Start(LoadTexture(OnSuccess, OnFail));
        }

        protected override bool AddToLibrary()
        {
            return true;
        }
        
        public override object GetId()
        {
            return model;
        }

        private IEnumerator LoadTexture(Action OnSuccess, Action<Exception> OnFail)
        {
            unitySamplingMode = model.samplingMode;

            TextureWrapMode unityWrap = TextureWrapMode.Clamp;
            switch (model.wrap)
            {
                case DCLTextureModel.BabylonWrapMode.CLAMP:
                    unityWrap = TextureWrapMode.Clamp;
                    break;
                case DCLTextureModel.BabylonWrapMode.WRAP:
                    unityWrap = TextureWrapMode.Repeat;
                    break;
                case DCLTextureModel.BabylonWrapMode.MIRROR:
                    unityWrap = TextureWrapMode.Mirror;
                    break;
            }

            if (texture == null && !string.IsNullOrEmpty(model.src))
            {
                bool isBase64 = model.src.Contains("image/png;base64");

                if (isBase64)
                {
                    string base64Data = model.src.Substring(model.src.IndexOf(',') + 1);

                    // The used texture variable can't be null for the ImageConversion.LoadImage to work
                    if (texture == null)
                    {
                        texture = new Texture2D(1, 1);
                    }

                    if (!ImageConversion.LoadImage(texture, Convert.FromBase64String(base64Data)))
                    {
                        string error = $"DCLTexture with id {GetId()} couldn't parse its base64 image data.";
                        Debug.LogError(error);
                        OnFail?.Invoke(new Exception(error));
                    }

                    if (texture != null)
                    {
                        texture.wrapMode = unityWrap;
                        texture.filterMode = unitySamplingMode;
                        texture.Compress(false);
                        texture.Apply(unitySamplingMode != FilterMode.Point, true);
                        
                        OnSuccess?.Invoke();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(contenstUrl))
                    {
                        if (texturePromise != null)
                            AssetPromiseKeeper_Texture.i.Forget(texturePromise);

                        texturePromise = new AssetPromise_Texture(contenstUrl, unityWrap, unitySamplingMode, storeDefaultTextureInAdvance: true);
                        texturePromise.OnSuccessEvent += (x) =>
                        {
                            texture = x.texture;
                            OnSuccess?.Invoke();
                        };
                        texturePromise.OnFailEvent += (x, error) => {
                        {
                            texture = null;
                            OnFail?.Invoke(error);
                        } };

                        AssetPromiseKeeper_Texture.i.Keep(texturePromise);
                        yield return texturePromise;
                    }
                }
            }
        }
    }
}