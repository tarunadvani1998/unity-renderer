using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DCL;
using DCL.Components;
using UnityEngine;

public interface IResourcePromiseKeeperService : IService
{
    UniTask<Resource_Material> GetMaterial(PBRMaterial.Model model);
    
    void ForgetMaterial(PBRMaterial.Model model);
}

public class ResourcePromiseKeeperService : IResourcePromiseKeeperService
{
    private ResourcePromiseKeeper_PBRMaterial materialPromiseKeeper;
    private IResourcePromiseKeeper_DCLTexture dclTexturePromiseKeeper;

    public ResourcePromiseKeeperService()
    {
        dclTexturePromiseKeeper = new ResourcePromiseKeeper_DCLTexture(); 
        materialPromiseKeeper = new ResourcePromiseKeeper_PBRMaterial(dclTexturePromiseKeeper);
    }
    
    public void Initialize()
    {
        dclTexturePromiseKeeper.Initialize();
        materialPromiseKeeper.Initialize();
    }
    
    public void Dispose()
    {
        materialPromiseKeeper.Dispose();
        dclTexturePromiseKeeper.Dispose();
    }
    
    public UniTask<Resource_Material> GetMaterial(PBRMaterial.Model model)
    {
        return materialPromiseKeeper.GetResource(model);
    }
    
    public void ForgetMaterial(PBRMaterial.Model model) { materialPromiseKeeper.ForgetResource(model); }

    public UniTask<Resource_DCLTexture> GetDCLTexture(DCLTextureModel model)
    {
        return dclTexturePromiseKeeper.GetResource(model);
    }
}
