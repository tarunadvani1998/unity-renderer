using DCL.Components;
using DCL.Controllers;
using DCL.Models;
using System;
using System.Collections;
using DCL.Helpers;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DCL
{
    public class DCLTexture : BaseDisposable,IDCLTexture
    {
        private Dictionary<ISharedComponent, HashSet<string>> attachedEntitiesByComponent =
            new Dictionary<ISharedComponent, HashSet<string>>();

        public TextureWrapMode unityWrap;
        public FilterMode unitySamplingMode;
        public Texture2D texture { get; protected set; }
        
        protected bool isDisposed;

        public override int GetClassId() { return (int) CLASS_ID.TEXTURE; }

        public DCLTexture() { model = new DCLTextureModel(); }

        public static IEnumerator FetchFromComponent(IParcelScene scene, string componentId,
            System.Action<Texture2D> OnFinish)
        {
            yield return FetchTextureComponent(scene, componentId, (dclTexture) => { OnFinish?.Invoke(dclTexture.texture); });
        }

        public static IEnumerator FetchTextureComponent(IParcelScene scene, string componentId,
            System.Action<DCLTexture> OnFinish)
        {
            if (!scene.disposableComponents.ContainsKey(componentId))
            {
                Debug.Log($"couldn't fetch texture, the DCLTexture component with id {componentId} doesn't exist");
                yield break;
            }

            DCLTexture textureComponent = scene.disposableComponents[componentId] as DCLTexture;

            if (textureComponent == null)
            {
                Debug.Log($"couldn't fetch texture, the shared component with id {componentId} is NOT a DCLTexture");
                yield break;
            }

            yield return new WaitUntil(() => textureComponent.texture != null);

            OnFinish.Invoke(textureComponent);
        }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            yield return new WaitUntil(() => CommonScriptableObjects.rendererState.Get());

            //If the scene creates and destroy the component before our renderer has been turned on bad things happen!
            //TODO: Analyze if we can catch this upstream and stop the IEnumerator
            if (isDisposed)
                yield break;

            DCLTextureModel model = (DCLTextureModel) newModel;
        }

        public virtual void AttachTo(ISharedComponent component)
        {
            AddReference(component);
        }

        public virtual void DetachFrom(ISharedComponent component)
        {
            RemoveReference(component);
        }

        public void AddReference(ISharedComponent component)
        {
            if (attachedEntitiesByComponent.ContainsKey(component))
                return;

            attachedEntitiesByComponent.Add(component, new HashSet<string>());

            foreach (var entity in component.GetAttachedEntities())
            {
                attachedEntitiesByComponent[component].Add(entity.entityId);
                DataStore.i.sceneWorldObjects.AddTexture(scene.sceneData.id, entity.entityId, texture);
            }
        }

        public void RemoveReference(ISharedComponent component)
        {
            if (!attachedEntitiesByComponent.ContainsKey(component))
                return;

            foreach (var entityId in attachedEntitiesByComponent[component])
            {
                DataStore.i.sceneWorldObjects.RemoveTexture(scene.sceneData.id, entityId, texture);
            }

            attachedEntitiesByComponent.Remove(component);
        }

        public override void Dispose()
        {
            if (isDisposed)
                return;
            
            isDisposed = true;

            while (attachedEntitiesByComponent.Count > 0)
            {
                RemoveReference(attachedEntitiesByComponent.First().Key);
            }
            base.Dispose();
        }
    }
}