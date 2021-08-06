using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarEditorHUDParticleSystemHandler : MonoBehaviour
{
    [SerializeField]
    HUDParticleController particleController;

    [SerializeField]
    AvatarEditorHUDView view;

    private void Start() {
        view.OnSetVisibility += OnSetAvatarEditorVisibility;
    }

    void OnSetAvatarEditorVisibility(bool visible)
    {
        if (visible) {
            view.OnAvatarAppear += OnAvatarAppear;
        } else {
            view.OnAvatarAppear -= OnAvatarAppear;
        }
    }

    void OnAvatarAppear(AvatarModel model) {
        if (!view.isOpen)
            return;

        particleController.StartEmitting();
    }
}