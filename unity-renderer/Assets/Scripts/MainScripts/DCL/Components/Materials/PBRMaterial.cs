using System;
using DCL.Controllers;
using DCL.Helpers;
using DCL.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCL.Components
{
    public class PBRMaterial : BaseDisposable
    {
        enum TransparencyMode
        {
            OPAQUE,
            ALPHA_TEST,
            ALPHA_BLEND,
            ALPHA_TEST_AND_BLEND,
            AUTO
        }

        public Material material { get; set; }
        private string currentMaterialResourcesFilename;

        const string MATERIAL_RESOURCES_PATH = "Materials/";
        const string PBR_MATERIAL_NAME = "ShapeMaterial";

        DCLTexture albedoDCLTexture = null;
        DCLTexture alphaDCLTexture = null;
        DCLTexture emissiveDCLTexture = null;
        DCLTexture bumpDCLTexture = null;

        private PBRMaterialModel oldModel;

        private List<Coroutine> textureFetchCoroutines = new List<Coroutine>();

        public PBRMaterial()
        {
            model = new PBRMaterialModel();

            OnAttach += OnMaterialAttached;
            OnDetach += OnMaterialDetached;
        }

        new public PBRMaterialModel GetModel() { return (PBRMaterialModel) model; }

        public override int GetClassId() { return (int) CLASS_ID.PBR_MATERIAL; }

        public override void AttachTo(IDCLEntity entity, System.Type overridenAttachedType = null)
        {
            if (attachedEntities.Contains(entity))
            {
                return;
            }

            entity.RemoveSharedComponent(typeof(BasicMaterial));
            base.AttachTo(entity);
        }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            PBRMaterialModel model = (PBRMaterialModel) newModel;

            // Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().ForgetMaterial(oldModel);
            oldModel = model;
            AsignMaterial(model);

            // Note: Ugly wait to insert Unitask in the components
            yield return new WaitUntil(() => material != null);
            
            foreach (IDCLEntity entity in attachedEntities)
                InitMaterial(entity);
        }

        private async void AsignMaterial(PBRMaterialModel model)
        {
            var wrapper = await Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().GetMaterial(model);
            material = wrapper.Get();
        }

        void OnMaterialAttached(IDCLEntity entity)
        {
            entity.OnShapeUpdated -= OnShapeUpdated;
            entity.OnShapeUpdated += OnShapeUpdated;

            if (entity.meshRootGameObject != null)
            {
                var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

                if (meshRenderer != null)
                    InitMaterial(entity);
            }
        }

        void InitMaterial(IDCLEntity entity)
        {
            var meshGameObject = entity.meshRootGameObject;

            if (meshGameObject == null)
                return;

            var meshRenderer = meshGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                return;

            PBRMaterialModel model = (PBRMaterialModel) this.model;

            meshRenderer.shadowCastingMode = model.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

            if (meshRenderer.sharedMaterial == material)
                return;

            MaterialTransitionController
                matTransition = meshGameObject.GetComponent<MaterialTransitionController>();

            if (matTransition != null && matTransition.canSwitchMaterial)
            {
                matTransition.finalMaterials = new Material[] { material };
                matTransition.PopulateTargetRendererWithMaterial(matTransition.finalMaterials);
            }

            Material oldMaterial = meshRenderer.sharedMaterial;
            meshRenderer.sharedMaterial = material;
            SRPBatchingHelper.OptimizeMaterial(material);

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, oldMaterial);
            DataStore.i.sceneWorldObjects.AddMaterial(scene.sceneData.id, entity.entityId, material);
        }

        private void OnShapeUpdated(IDCLEntity entity)
        {
            if (entity != null)
                InitMaterial(entity);
        }

        private void OnMaterialDetached(IDCLEntity entity)
        {
            if (entity.meshRootGameObject == null)
                return;

            entity.OnShapeUpdated -= OnShapeUpdated;

            var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer && meshRenderer.sharedMaterial == material)
                meshRenderer.sharedMaterial = null;

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, material);
        }

        public override void Dispose()
        {
            albedoDCLTexture?.DetachFrom(this);
            alphaDCLTexture?.DetachFrom(this);
            emissiveDCLTexture?.DetachFrom(this);
            bumpDCLTexture?.DetachFrom(this);

            for (int i = 0; i < textureFetchCoroutines.Count; i++)
            {
                var coroutine = textureFetchCoroutines[i];

                if ( coroutine != null )
                    CoroutineStarter.Stop(coroutine);
            }
            
            // Environment.i.serviceLocator.Get<IResourcePromiseKeeperService>().ForgetMaterial(oldModel);

            base.Dispose();
        }
    }
}