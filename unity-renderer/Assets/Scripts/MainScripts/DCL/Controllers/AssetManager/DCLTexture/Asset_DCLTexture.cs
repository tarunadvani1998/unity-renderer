using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityGLTF.Cache;
using Object = UnityEngine.Object;

namespace DCL
{
    public class Asset_DCLTexture : Asset
    {
        public DCLTexture texture { get; set; }
        public event System.Action OnCleanup;

        public override void Cleanup()
        {
            OnCleanup?.Invoke();
            PersistentAssetCache.RemoveImage(texture.texture);
            // Object.Destroy(texture);
        }

        public void Dispose() { Cleanup(); }
    }
}