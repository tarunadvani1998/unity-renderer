using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DCL;
using UnityEngine;
using WaitUntil = UnityEngine.WaitUntil;

public interface IResourcePromiseKeeper_DCLTexture: IResourcePromiseKeeper<DCLTextureModel, Resource_DCLTexture>
{
    IEnumerator FetchTextureComponent(string componentId,
        System.Action<DCLTexture> OnFinish);
}

public class ResourcePromiseKeeper_DCLTexture : ResourcePromiseKeeper<DCLTextureModel, Resource_DCLTexture>, IResourcePromiseKeeper_DCLTexture
{
    protected override async UniTask<Resource_DCLTexture> CreateResource(DCLTextureModel baseModel)
    {
        UniTask<Resource_DCLTexture> task = new UniTask<Resource_DCLTexture>();
        Resource_DCLTexture resourceDclTexture = new Resource_DCLTexture();
        
        string contentsUrl = string.Empty;
        bool isExternalURL = baseModel.src.Contains("http://") || baseModel.src.Contains("https://");

        if (isExternalURL)
            contentsUrl = baseModel.src;
        else
            contentsUrl = FindSceneForTheDCLTexture(baseModel);

        Promise<DCLTexture> promise = new Promise<DCLTexture>();
        
        AssetPromise_DCLTexture promiseKeeperDclTexture = new AssetPromise_DCLTexture(baseModel,contentsUrl);
        promiseKeeperDclTexture.OnSuccessEvent += texture => promise.Resolve(texture);
        promiseKeeperDclTexture.OnFailEvent += (texture, error) => promise.Catch(error);
        AssetPromiseKeeper_DCLTexture.i.Keep(promiseKeeperDclTexture);
        
        DCLTexture dclTexture = null;
        dclTexture.UpdateFromModel(baseModel);

        
        promise.Then(texture =>
        {
            dclTexture = texture;
        });
        promise.Catch( error =>
        {
            dclTexture = null;
        });
        
        await promise;
        resourceDclTexture.Set(dclTexture);
        return resourceDclTexture;
    }

    private string FindSceneForTheDCLTexture(DCLTextureModel baseModel)
    {
        // foreach (IParcelScene scene in Environment.i.world.state.scenesSortedByDistance)
        // {
        //     if(!scene.disposableComponents.TryGetValue(CLASS_ID.TEXTURE.ToString(), out ISharedComponent sharedComponent))
        //         continue;
        //     
        //     DCLTexture textureComponent = (DCLTexture) sharedComponent;
        //     if(textureComponent.GetModel() != baseModel)
        //         continue;
        //     
        //     string contentsUrl;
        //     scene.contentProvider.TryGetContentsUrl(baseModel.src, out contentsUrl);
        //     return contentsUrl;
        // }
        return "";
    }
    
    public IEnumerator FetchTextureComponent(string componentId,
        System.Action<DCLTexture> OnFinish)
    {
        // // NOTE: This will dissapear in the next PR where the DCLTextures won't be attached to an entity, or scene
        // foreach (IParcelScene scene in Environment.i.world.state.scenesSortedByDistance)
        // {
        //     if(!scene.disposableComponents.TryGetValue(componentId, out ISharedComponent sharedComponent))
        //         continue;
        //     
        //     DCLTexture textureComponent = (DCLTexture) sharedComponent;
        //
        //     if (textureComponent == null)
        //     {
        //         Debug.Log($"couldn't fetch texture, the shared component with id {componentId} is NOT a DCLTexture");
        //         yield break;
        //     }
        //
        //     yield return new WaitUntil(() => textureComponent.texture != null);
        //
        //     OnFinish.Invoke(textureComponent);
        // }
        //
        // if (Environment.i.world.state.scenesSortedByDistance[0].disposableComponents.TryGetValue(componentId, out ISharedComponent sharedComponent2))
        // {
        //
        //     DCLTexture textureComponent = (DCLTexture) sharedComponent2;
        //
        // if (textureComponent == null)
        // {
        //     Debug.Log($"couldn't fetch texture, the shared component with id {componentId} is NOT a DCLTexture");
        //     yield break;
        // }
        //
        // yield return new WaitUntil(() => textureComponent.texture != null);
        //
        // OnFinish.Invoke(textureComponent);
        //                 
        // }

        yield return  null;
    }
}
