using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DCL;
using DCL.Components;
using UnityEngine;
using UnityEngine.Diagnostics;

public class ResourcePromiseKeeper_PBRMaterial : ResourcePromiseKeeper<PBRMaterialModel,Resource_Material>
{
    public ResourcePromiseKeeper_PBRMaterial(IResourcePromiseKeeper_DCLTexture texturePromiseKeeper)
    {
        // this.texturePromiseKeeper = texturePromiseKeeper;
    }

    protected override async UniTask<Resource_Material> CreateResource(PBRMaterialModel baseModel)
    {
        Resource_Material resourceMaterial = new Resource_Material();
        // Material material = await SetupMaterial(baseModel);
        // resourceMaterial.Set(material);
        return resourceMaterial;
    }

    
}
