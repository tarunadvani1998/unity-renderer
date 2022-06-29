using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DCL.Components
{
    public class RendereableAssetLoadHelper
    {
        public enum LoadingType
        {
            ASSET_BUNDLE_WITH_GLTF_FALLBACK,
            ASSET_BUNDLE_WITH_OLD_GLTF_FALLBACK,
            ASSET_BUNDLE_ONLY,
            GLTF_ONLY,
            OLD_GLTF,
            DEFAULT
        }

        public static bool VERBOSE = false;

        public static bool useCustomContentServerUrl = false;
        public static string customContentServerUrl;

        public static LoadingType defaultLoadingType = LoadingType.ASSET_BUNDLE_WITH_GLTF_FALLBACK;

        public AssetPromiseSettings_Rendering settings = new AssetPromiseSettings_Rendering();

        public Rendereable loadedAsset { get; protected set; }

        public bool isFinished
        {
            get
            {
                if (gltfPromise != null)
                    return gltfPromise.state == AssetPromiseState.FINISHED;

                if (abPromise != null)
                    return abPromise.state == AssetPromiseState.FINISHED;

                return true;
            }
        }

        string bundlesContentUrl;
        private readonly Func<bool> checkIfGltFastIsEnabled;
        ContentProvider contentProvider;

        AssetPromise_GLTF gltfPromise;
        AssetPromise_GLTFast_GameObject gltfastPromise;
        AssetPromise_AB_GameObject abPromise;

#if UNITY_EDITOR
        public override string ToString()
        {
            float loadTime = Mathf.Min(loadFinishTime, Time.realtimeSinceStartup) - loadStartTime;

            string result = "not loading";

            if (gltfPromise != null)
            {
                result = $"GLTF -> promise state = {gltfPromise.state} ({loadTime} load time)... waiting promises = {AssetPromiseKeeper_GLTF.i.waitingPromisesCount}";

                if (gltfPromise.state == AssetPromiseState.WAITING)
                {
                    result += $"\nmaster promise state... is blocked... {AssetPromiseKeeper_GLTF.i.GetMasterState(gltfPromise)}";
                }
            }

            if (abPromise != null)
            {
                result = $"ASSET BUNDLE -> promise state = {abPromise.ToString()} ({loadTime} load time)... waiting promises = {AssetPromiseKeeper_AB.i.waitingPromisesCount}";
            }

            return result;
        }

        float loadStartTime = 0;
        float loadFinishTime = float.MaxValue;
#endif

        public RendereableAssetLoadHelper( ContentProvider contentProvider, string bundlesContentUrl, Func<bool> checkIfGltFastIsEnabled)
        {
            this.contentProvider = contentProvider;
            this.bundlesContentUrl = bundlesContentUrl;
            this.checkIfGltFastIsEnabled = checkIfGltFastIsEnabled;
        }

        public event Action<Rendereable> OnSuccessEvent;
        public event Action<Exception> OnFailEvent;

        public void Load(string targetUrl, LoadingType forcedLoadingType = LoadingType.DEFAULT)
        {
            Assert.IsFalse(string.IsNullOrEmpty(targetUrl), "url is null!!");
#if UNITY_EDITOR
            loadStartTime = Time.realtimeSinceStartup;
#endif

            LoadingType finalLoadingType = forcedLoadingType == LoadingType.DEFAULT ? defaultLoadingType : forcedLoadingType;
            switch (finalLoadingType)
            {
                case LoadingType.ASSET_BUNDLE_ONLY:
                    LoadAssetBundle(targetUrl, OnSuccessEvent, OnFailEvent);
                    break;
                case LoadingType.GLTF_ONLY:
                    ProxyLoadGltf(targetUrl);
                    break;
                case LoadingType.OLD_GLTF:
                    LoadGltf(targetUrl, OnSuccessEvent, OnFailEvent);
                    break;
                case LoadingType.DEFAULT:
                case LoadingType.ASSET_BUNDLE_WITH_GLTF_FALLBACK:
                    LoadAssetBundle(targetUrl, OnSuccessEvent, exception => ProxyLoadGltf(targetUrl));
                    break;                
                case LoadingType.ASSET_BUNDLE_WITH_OLD_GLTF_FALLBACK:
                    LoadAssetBundle(targetUrl, OnSuccessEvent, exception => LoadGltf(targetUrl, OnSuccessEvent, OnFailEvent));
                    break;
            }
        }
        private void ProxyLoadGltf(string targetUrl)
        {
            // TODO: Replace with feature flag
            bool shouldUseGLTFast = checkIfGltFastIsEnabled();
            
            if (shouldUseGLTFast)
            {
                LoadGLTFast(targetUrl, OnSuccessEvent, exception =>
                {
                    LoadGltf(targetUrl, OnSuccessEvent, OnFailEvent);
                });
            }
            else
            {
                LoadGltf(targetUrl, OnSuccessEvent, OnFailEvent);
            }
        }

        public void Unload()
        {
            UnloadAB();
            UnloadGLTF();
            UnloadGLTFast();
        }

        void UnloadAB()
        {
            if ( abPromise != null )
            {
                AssetPromiseKeeper_AB_GameObject.i.Forget(abPromise);
            }
        }

        void UnloadGLTF()
        {
            if ( gltfPromise != null )
            {
                AssetPromiseKeeper_GLTF.i.Forget(gltfPromise);
            }
        }
        
        void UnloadGLTFast()
        {
            if ( gltfastPromise != null )
            {
                AssetPromiseKeeper_GLTFast_GameObject.i.Forget(gltfastPromise);
            }
        }

        private const string AB_GO_NAME_PREFIX = "AB:";
        private const string GLTF_GO_NAME_PREFIX = "GLTF:";

        void LoadAssetBundle(string targetUrl, Action<Rendereable> OnSuccess, Action<Exception> OnFail)
        {
            if (abPromise != null)
            {
                UnloadAB();
                if (VERBOSE)
                    Debug.Log("Forgetting not null promise..." + targetUrl);
            }

            string bundlesBaseUrl = useCustomContentServerUrl ? customContentServerUrl : bundlesContentUrl;

            if (string.IsNullOrEmpty(bundlesBaseUrl))
            {
                OnFailWrapper(OnFail, new Exception("bundlesBaseUrl is null"));
                return;
            }

            if (!contentProvider.TryGetContentsUrl_Raw(targetUrl, out string hash))
            {
                OnFailWrapper(OnFail, new Exception($"Content url does not contains {targetUrl}"));
                return;
            }

            abPromise = new AssetPromise_AB_GameObject(bundlesBaseUrl, hash);
            abPromise.settings = this.settings;

            abPromise.OnSuccessEvent += (x) =>
            {
#if UNITY_EDITOR
                x.container.name = AB_GO_NAME_PREFIX + x.container.name; 
#endif
                var r = new Rendereable()
                {
                    container = x.container,
                    totalTriangleCount = x.totalTriangleCount,
                    meshes = x.meshes,
                    renderers = x.renderers,
                    materials = x.materials,
                    textures = x.textures,
                    meshToTriangleCount = x.meshToTriangleCount,
                    animationClipSize = x.animationClipSize,
                    animationClips = x.animationClips,
                    meshDataSize = x.meshDataSize
                };

                OnSuccessWrapper(r, OnSuccess);
            };

            abPromise.OnFailEvent += (x, exception) => OnFailWrapper(OnFail, exception);

            AssetPromiseKeeper_AB_GameObject.i.Keep(abPromise);
        }

        void LoadGltf(string targetUrl, Action<Rendereable> OnSuccess, Action<Exception> OnFail)
        {
            if (gltfPromise != null)
            {
                UnloadGLTF();

                if (VERBOSE)
                    Debug.Log("Forgetting not null promise... " + targetUrl);
            }

            if (!contentProvider.TryGetContentsUrl_Raw(targetUrl, out string hash))
            {
                OnFailWrapper(OnFail, new Exception($"Content provider does not contains url {targetUrl}"));
                return;
            }

            gltfPromise = new AssetPromise_GLTF(contentProvider, targetUrl, hash);
            gltfPromise.settings = settings;

            gltfPromise.OnSuccessEvent += (Asset_GLTF x) =>
            {
#if UNITY_EDITOR
                x.container.name = GLTF_GO_NAME_PREFIX + x.container.name;
#endif
                var r = new Rendereable
                {
                    container = x.container,
                    totalTriangleCount = x.totalTriangleCount,
                    meshes = x.meshes,
                    renderers = x.renderers,
                    materials = x.materials,
                    textures = x.textures,
                    meshToTriangleCount = x.meshToTriangleCount,
                    animationClipSize = x.animationClipSize,
                    meshDataSize = x.meshDataSize,
                    animationClips = x.animationClips
                };

                OnSuccessWrapper(r, OnSuccess);
            };
            gltfPromise.OnFailEvent += (asset, exception) => OnFailWrapper(OnFail, exception);

            AssetPromiseKeeper_GLTF.i.Keep(gltfPromise);
        }
        private void LoadGLTFast(string targetUrl, Action<Rendereable> OnSuccess, Action<Exception> OnFail)
        {
            if (gltfastPromise != null)
            {
                UnloadGLTFast();

                if (VERBOSE)
                    Debug.Log("Forgetting not null promise... " + targetUrl);
            }

            if (!contentProvider.TryGetContentsUrl_Raw(targetUrl, out string hash))
            {
                OnFailWrapper(OnFail, new Exception($"Content provider does not contains url {targetUrl}"));
                return;
            }

            gltfastPromise = new AssetPromise_GLTFast_GameObject(targetUrl, hash, contentProvider)
            {
                settings = settings
            };

            gltfastPromise.OnSuccessEvent += (Asset_GLTFast_GameObject x) =>
            {
#if UNITY_EDITOR
                x.container.name = GLTF_GO_NAME_PREFIX + x.container.name;
#endif
                Rendereable r = x.ToRendereable();

                OnSuccessWrapper(r, OnSuccess);
            };
            gltfastPromise.OnFailEvent += (asset, exception) =>
            {
                Debug.LogException(exception);
                OnFailWrapper(OnFail, exception);
            };

            AssetPromiseKeeper_GLTFast_GameObject.i.Keep(gltfastPromise);
        }

        private void OnFailWrapper(Action<Exception> OnFail, Exception exception)
        {
#if UNITY_EDITOR
            loadFinishTime = Time.realtimeSinceStartup;
#endif

            OnFail?.Invoke(exception);
            ClearEvents();
        }

        private void OnSuccessWrapper(Rendereable loadedAsset, Action<Rendereable> OnSuccess)
        {
#if UNITY_EDITOR
            loadFinishTime = Time.realtimeSinceStartup;
#endif
            if (VERBOSE)
            {
                if (gltfPromise != null)
                    Debug.Log($"GLTF Load(): target URL -> {gltfPromise.GetId()}. Success!");
                else
                    Debug.Log($"AB Load(): target URL -> {abPromise.hash}. Success!");
            }

            this.loadedAsset = loadedAsset;
            OnSuccess?.Invoke(loadedAsset);
            ClearEvents();
        }

        public void ClearEvents()
        {
            OnSuccessEvent = null;
            OnFailEvent = null;
        }
    }
}