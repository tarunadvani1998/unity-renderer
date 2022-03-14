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

        Texture2D newTexture = new Texture2D(512, 512);

        Rendereable r = new Rendereable();
        r.textures.Add(newTexture);

        DataStore.i.sceneWorldObjects.AddRendereable(currentSceneId, r);
    }

    void Update()
    {
        if (Environment.i == null)
            return;

        if (Time.frameCount % 60 != 0)
            return;

        SceneMetricsModel model = new SceneMetricsModel();

        foreach (var kvp in Environment.i.world.state.loadedScenes)
        {
            var sceneData = kvp.Value.metricsCounter.currentCount;
            model += sceneData;
        }

        text.text = "";
        text.text += $"Total Memory: {ToMb(model.totalMemoryScore)} (Profiler: {ToMb(model.totalMemoryProfiler)})\n\n";
        text.text += $"Textures: {ToMb(model.textureMemoryScore)} (Profiler: {ToMb(model.textureMemoryProfiler)}\n";
        text.text +=
            $"Animations: {ToMb(model.animationClipMemoryScore)} (Profiler: {ToMb(model.animationClipMemoryProfiler)})\n";
        text.text += $"Meshes: {ToMb(model.meshMemoryScore)} (Profiler: {ToMb(model.meshMemoryProfiler)})\n";
        text.text +=
            $"AudioClips: {ToMb(model.audioClipMemoryScore)} (Profiler: {ToMb(model.audioClipMemoryProfiler)})\n";
    }

    string ToMb(long memory)
    {
        double mb = 1024 * 1024;
        return (memory / mb).ToString("N") + "Mb";
    }
}