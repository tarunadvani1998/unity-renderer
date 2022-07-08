﻿using DCL.Controllers;
using DCL.ECSRuntime;
using DCL.Models;

namespace DCL.ECSComponents
{
    /// <summary>
    /// This class will handle the avatar shape for the global scenes and the scenes that use it
    /// Take into account that this component uses a pool to manage the prefabs
    /// </summary>
    public class AvatarShapeComponentHandler : IECSComponentHandler<PBAvatarShape>
    {
        internal IAvatarShape avatar;
        private readonly Pool pool;
        private readonly PoolableObject poolableObject;

        public AvatarShapeComponentHandler(Pool pool)
        {
            this.pool = pool;
            poolableObject = pool.Get(); 
            avatar = poolableObject.gameObject.GetComponent<AvatarShape>();
        }
        
        public void OnComponentCreated(IParcelScene scene, IDCLEntity entity)
        {
            avatar.transform.SetParent(entity.gameObject.transform);
        }

        public void OnComponentRemoved(IParcelScene scene, IDCLEntity entity)
        {
            if (avatar == null)
                return;
            
            avatar.Cleanup();
            pool.Release(poolableObject);
            avatar = null;
        }

        public void OnComponentModelUpdated(IParcelScene scene, IDCLEntity entity, PBAvatarShape model)
        {
            avatar.ApplyModel(scene,entity,model);
        }
    }
}