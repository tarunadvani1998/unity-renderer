using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GPUSkinning;
using UnityEngine;

namespace AvatarSystem
{
    public class Avatar : IAvatar
    {
        private readonly IAvatarCurator avatarCurator;
        private readonly ILoader loader;
        private readonly IAnimator animator;
        private readonly IVisibility visibility;
        private readonly ILOD lod;
        private readonly IGPUSkinning gpuSkinning;
        private readonly IGPUSkinningThrottler gpuSkinningThrottler;
        private CancellationTokenSource disposeCts = new CancellationTokenSource();

        public IAvatar.Status status { get; private set; } = IAvatar.Status.Idle;
        public Vector3 extents { get; private set; }

        public Avatar(IAvatarCurator avatarCurator, ILoader loader, IAnimator animator, IVisibility visibility, ILOD lod, IGPUSkinning gpuSkinning, IGPUSkinningThrottler gpuSkinningThrottler)
        {
            this.avatarCurator = avatarCurator;
            this.loader = loader;
            this.animator = animator;
            this.visibility = visibility;
            this.lod = lod;
            this.gpuSkinning = gpuSkinning;
            this.gpuSkinningThrottler = gpuSkinningThrottler;
        }

        /// <summary>
        /// Starts the loading process for the Avatar. 
        /// </summary>
        /// <param name="wearablesIds"></param>
        /// <param name="settings"></param>
        /// <param name="ct"></param>
        public async UniTask Load(List<string> wearablesIds, AvatarSettings settings, CancellationToken ct = default)
        {
            status = IAvatar.Status.Idle;
            CancellationToken linkedCt = CancellationTokenSource.CreateLinkedTokenSource(ct, disposeCts.Token).Token;

            linkedCt.ThrowIfCancellationRequested();

            try
            {
                visibility.SetLoadingReady(false);
                WearableItem bodyshape = null;
                WearableItem eyes = null;
                WearableItem eyebrows = null;
                WearableItem mouth = null;
                List<WearableItem> wearables = null;

                (bodyshape, eyes, eyebrows, mouth, wearables) = await avatarCurator.Curate(settings.bodyshapeId , wearablesIds, linkedCt);

                await loader.Load(bodyshape, eyes, eyebrows, mouth, wearables, settings, linkedCt);

                extents = loader.combinedRenderer.localBounds.extents * 2f / 100f;

                animator.Prepare(settings.bodyshapeId, loader.bodyshapeContainer);

                gpuSkinning.Prepare(loader.combinedRenderer);
                gpuSkinningThrottler.Bind(gpuSkinning);

                visibility.Bind(gpuSkinning.renderer,  new [] { loader.eyesRenderer, loader.eyebrowsRenderer, loader.mouthRenderer });
                visibility.SetLoadingReady(true);

                lod.Bind(gpuSkinning.renderer, new [] { loader.eyesRenderer, loader.eyebrowsRenderer, loader.mouthRenderer });
                gpuSkinningThrottler.Start();

                status = IAvatar.Status.Loaded;
            }
            catch (OperationCanceledException)
            {
                Dispose();
            }
            catch (Exception e)
            {
                Dispose();
                Debug.LogError($"Avatar.Load failed with wearables:[{string.Join(",", wearablesIds)}] for bodyshape:{settings.bodyshapeId} and player {settings.playerName}");
                Debug.LogException(e);
                throw;
            }
        }

        public void SetVisibility(bool visible) { visibility.SetExplicitVisibility(visible); }

        public void SetExpression(string expressionId, long timestamps) { animator?.PlayExpression(expressionId, timestamps); }

        public void SetLODLevel(int lodIndex) { lod.SetLodIndex(lodIndex); }

        public void SetAnimationThrottling(int framesBetweenUpdate) { gpuSkinningThrottler.SetThrottling(framesBetweenUpdate); }

        public void SetImpostorTexture(Texture2D impostorTexture) { lod.SetImpostorTexture(impostorTexture); }
        public void SetImpostorTint(Color color) { lod.SetImpostorTint(color); }
        public Transform[] GetBones() => loader.GetBones();

        public void Dispose()
        {
            status = IAvatar.Status.Idle;
            disposeCts?.Cancel();
            disposeCts = new CancellationTokenSource();
            avatarCurator?.Dispose();
            loader?.Dispose();
            visibility?.Dispose();
            lod?.Dispose();
            gpuSkinningThrottler?.Dispose();
        }
    }
}