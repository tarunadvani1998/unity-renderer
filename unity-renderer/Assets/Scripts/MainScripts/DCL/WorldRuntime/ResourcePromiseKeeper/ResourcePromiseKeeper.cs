using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IResourcePromiseKeeper<Y,T>  where Y : BaseModel
{
    void Initialize();
    
    void Dispose();

    UniTask<T> GetResource(Y model);

    void ForgetResource(Y model);
}

public abstract class ResourcePromiseKeeper<Y,T> : IResourcePromiseKeeper<Y,T> where Y : BaseModel where T : Resource
{
    protected Dictionary<Y, T> resourceDictionary;
    protected Dictionary<Y, int> usageDictionary;

    public void Initialize()
    {
        resourceDictionary = new Dictionary<Y, T>();
        usageDictionary = new Dictionary<Y, int>();
    }

    public void Dispose()
    {
        resourceDictionary.Clear();
        usageDictionary.Clear();
    }

    public virtual async UniTask<T> GetResource(Y model)
    {
        IncrementUsage(model);
        if (resourceDictionary.ContainsKey(model))
            return resourceDictionary[model];
        
        T resource = await CreateResource(model);
        resourceDictionary.Add(model,resource);
        return resource;
    }

    public virtual void ForgetResource(Y model)
    {
        if(model == null)
            return;
        DecrementUsage(model);
    }

    protected abstract UniTask<T> CreateResource(Y baseModel);

    private void IncrementUsage(Y model)
    {
        if (usageDictionary.ContainsKey(model))
            usageDictionary[model]++;
        else
            usageDictionary.Add(model, 1);
    }

    private void DecrementUsage(Y model)
    {
        if (!usageDictionary.ContainsKey(model))
            Debug.LogError("Error trying to remove a resource that doesn't exits. This shouldn't happen. We have a bug here!");

        usageDictionary[model]--;
        if (usageDictionary[model] == 0)
            RemoveResource(model);
    }

    protected virtual void RemoveResource(Y model)
    {
        resourceDictionary[model].Destroy();
        resourceDictionary.Remove(model);
        usageDictionary.Remove(model);
    }
}
