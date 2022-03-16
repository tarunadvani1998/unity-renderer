using System.Collections;
using System.Collections.Generic;
using DCL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneMetricsHUD : MonoBehaviour
{
    public TMP_Text text;
    public Button allocateNewTexture;

    void Start()
    {
        allocateNewTexture.onClick.AddListener(AllocNewTexture);
    }

    private void AllocNewTexture()
    {
        var currentSceneId = Environment.i.world.state.currentSceneId;

        Texture2D newTexture = new Texture2D(1024, 1024);
        newTexture.Apply(true, true);
        
        Rendereable r = new Rendereable();
        r.textures.Add(newTexture);
        r.ownerId = "Blah";

        DataStore.i.sceneWorldObjects.AddRendereable(currentSceneId, r);

        forceUpdate = true;
    }

    private void AllocNewMesh()
    {
        var currentSceneId = Environment.i.world.state.currentSceneId;

        Mesh newMesh = new Mesh();

        Rendereable r = new Rendereable();
        r.meshes.Add(newMesh);
        r.ownerId = "Blah";

        DataStore.i.sceneWorldObjects.AddRendereable(currentSceneId, r);
        
        forceUpdate = true;
    }
    
    bool forceUpdate = false;

    void Update()
    {
        if (Environment.i == null)
            return;

        if (Time.frameCount % 60 != 0 && !forceUpdate)
            return;

        forceUpdate = false;

        SceneMetricsModel model = new SceneMetricsModel();

        foreach (var kvp in Environment.i.world.state.loadedScenes)
        {
            var sceneData = kvp.Value.metricsCounter.currentCount;
            model += sceneData;
        }

        text.text = "Global Stats:\n\n";

        text.text += $"Total Memory: {ToMb(model.totalMemoryScore)} (Profiler: {ToMb(model.totalMemoryProfiler)})\n\n";
        text.text += $"Textures (x{model.textures}): {ToMb(model.textureMemoryScore)} (Profiler: {ToMb(model.textureMemoryProfiler)}\n";
        text.text +=
            $"Animations: {ToMb(model.animationClipMemoryScore)} (Profiler: {ToMb(model.animationClipMemoryProfiler)})\n";
        text.text += $"Meshes (x{model.meshes}): {ToMb(model.meshMemoryScore)} (Profiler: {ToMb(model.meshMemoryProfiler)})\n";
        text.text +=
            $"AudioClips: {ToMb(model.audioClipMemoryScore)} (Profiler: {ToMb(model.audioClipMemoryProfiler)})\n\n";

        string currentSceneId = Environment.i.world.state.currentSceneId;

        if (string.IsNullOrEmpty(currentSceneId))
            return;

        var scene = Environment.i.world.state.loadedScenes[currentSceneId];
        model = scene.metricsCounter.currentCount;

        text.text +=
            $"Current Scene Stats ({scene.sceneData.basePosition}... parcel count: {scene.sceneData.parcels.Length}):\n\n";

        text.text += $"Total Memory: {ToMb(model.totalMemoryScore)} (Profiler: {ToMb(model.totalMemoryProfiler)})\n\n";
        text.text +=
            $"Textures (x{model.textures}): {ToMb(model.textureMemoryScore)} (Profiler: {ToMb(model.textureMemoryProfiler)}\n";
        text.text +=
            $"Animations: {ToMb(model.animationClipMemoryScore)} (Profiler: {ToMb(model.animationClipMemoryProfiler)})\n";
        text.text +=
            $"Meshes (x{model.meshes}): {ToMb(model.meshMemoryScore)} (Profiler: {ToMb(model.meshMemoryProfiler)})\n";
        text.text +=
            $"AudioClips: {ToMb(model.audioClipMemoryScore)} (Profiler: {ToMb(model.audioClipMemoryProfiler)})\n\n";
    }

    string ToMb(long memory)
    {
        double mb = 1024 * 1024;
        return (memory / mb).ToString("N") + "Mb";
    }
}