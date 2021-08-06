using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDParticleSystem : MonoBehaviour
{
    [System.Serializable]
    public class Collection
    {
        public HUDParticle template;
        public int amount = 10;
        [Space]
        public HUDParticle.Settings settings;

        RectTransform origin;
        Transform parent;

        public void Init(RectTransform origin, Transform parent) {
            this.origin = origin;
            this.parent = parent;
            template.gameObject.SetActive(false);
        }

        public void StartEmitting() {
            if (origin == null)
                return;

            for (int i = 0; i < amount; i++) {
                HUDParticle p = Instantiate(template, parent);
                p.Initialize(settings, origin);
            }
        }

        public void SetOrigin(RectTransform origin) {
            this.origin = origin;
        }
    }

    public Collection[] collections;

    private void Awake() {
        RectTransform rt = GetComponent<RectTransform>();

        for (int i = 0; i < collections.Length; i++) {
            collections[i].Init(rt, transform);
        }
    }

    public void StartEmitting() {
        for (int i = 0; i < collections.Length; i++) {
            collections[i].StartEmitting();
        }
    }

    public void SetOrigin(RectTransform origin) {
        for (int i = 0; i < collections.Length; i++) {
            collections[i].SetOrigin(origin);
        }
    }
}
