using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Categories = WearableLiterals.Categories;
using Rarity = WearableLiterals.ItemRarity;

public class AvatarEditorHUDParticleSystemHandler : MonoBehaviour
{
    [SerializeField]
    HUDParticleSystem particleController;

    [SerializeField]
    AvatarEditorHUDView view;

    [SerializeField]
    ItemSelector nftItemSelector;

    [SerializeField, Header("Origins")]
    RectTransform originFootwear;
    [SerializeField]
    RectTransform originLowerBody;
    [SerializeField]
    RectTransform originUpperBody;
    [SerializeField]
    RectTransform originHeadwear;

    bool wearableIsSameAsPrevious;
    WearableItem lastSelectedWearable = null;

    private void Start() {
        int nPairs = view.wearableGridPairs.Length;
        for (int i = 0; i < nPairs; i++) {
            view.wearableGridPairs[i].selector.OnItemClicked += OnSelectWearable;
        }

        nftItemSelector.OnItemClicked += OnSelectWearable;

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

    void OnSelectWearable(string wearableId) {
        CatalogController.wearableCatalog.TryGetValue(wearableId, out var wearable);
        wearableIsSameAsPrevious = (wearable == lastSelectedWearable);
        if (wearableIsSameAsPrevious)
            return;

        lastSelectedWearable = wearable;
        if (wearable == null)
            return;

        RectTransform origin = null;

        switch (wearable.data.category) {
            case Categories.EYEBROWS:
                origin = originHeadwear;
                break;
            case "facial_hair":
                origin = originHeadwear;
                break;
            case Categories.FEET:
                origin = originFootwear;
                break;
            case Categories.HAIR:
                origin = originHeadwear;
                break;
            case Categories.LOWER_BODY:
                origin = originLowerBody;
                break;
            case Categories.UPPER_BODY:
                origin = originUpperBody;
                break;
            case "eyewear":
                origin = originHeadwear;
                break;
            case "tiara":
                origin = originHeadwear;
                break;
            case "earring":
                origin = originHeadwear;
                break;
            case "hat":
                origin = originHeadwear;
                break;
            case "top_head":
                origin = originHeadwear;
                break;
            case "helmet":
                origin = originHeadwear;
                break;
            case "mask":
                origin = originHeadwear;
                break;
            default:
                break;
        }

        particleController.SetOrigin(origin);
    }

    void OnAvatarAppear(AvatarModel model) {
        if (!view.isOpen)
            return;

        particleController.StartEmitting();
    }
}