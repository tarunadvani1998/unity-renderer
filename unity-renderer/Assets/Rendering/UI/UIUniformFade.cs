using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUniformFade : MonoBehaviour
{
    [Range(0,1)]
    public float canvasVisibility = 1;

    public CanvasGroup canvas;
    void Update()
    {
        canvas.alpha = canvasVisibility;
    }
}
