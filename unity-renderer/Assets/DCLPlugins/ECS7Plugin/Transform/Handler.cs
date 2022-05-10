using DCL.Controllers;
using DCL.ECSRuntime;
using DCL.Helpers;
using DCL.Models;
using UnityEngine;

namespace DCLPlugins.Transform
{
    public class Handler : IECSComponentHandler<Model>
    {
        void IECSComponentHandler<Model>.OnComponentCreated(IParcelScene scene, IDCLEntity entity)
        {
            entity.EnsureMeshGameObject("box mesh");
            MeshFilter meshFilter = entity.meshRootGameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = entity.meshRootGameObject.AddComponent<MeshRenderer>();

            entity.meshesInfo.renderers = new Renderer[] { meshRenderer };
            meshFilter.sharedMesh = PrimitiveMeshBuilder.BuildCube(1f);
        }

        void IECSComponentHandler<Model>.OnComponentRemoved(IParcelScene scene, IDCLEntity entity)
        {
            Object.Destroy(entity.meshesInfo.meshRootGameObject);
            entity.meshesInfo.meshRootGameObject = null;
        }

        void IECSComponentHandler<Model>.OnComponentModelUpdated(IParcelScene scene, IDCLEntity entity, Model model)
        {
            entity.gameObject.transform.localPosition = model.position;
            entity.gameObject.transform.localRotation = model.rotation;
            entity.gameObject.transform.localScale = model.scale;
        }
    }
}