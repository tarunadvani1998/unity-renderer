using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDParticleController : MonoBehaviour
{
    public class Particle
    {
        [System.Serializable]
        public class Settings
        {
            public Vector2 lifetimeMinMax = Vector2.one;

            [Space]
            public Vector2 scaleMinMax = Vector2.one;
            public AnimationCurve scaleOverLifetime = AnimationCurve.Constant(0f, 1f, 1f);

            [Space]
            public Vector2 speedMinMax = Vector2.one * 300;
            public AnimationCurve speedOverLifetime = AnimationCurve.Constant(0f, 1f, 1f);

            [Space]
            public AnimationCurve alphaOverLifetime = AnimationCurve.Linear(0f, 1f, 1f, 0f);

            public float GetRandomRange(Vector2 range) {
                return Random.Range(range.x, range.y);
            }
        }

        public RectTransform rt;
        public float timer, direction, speed;

        Settings settings;
        Image image;
        bool disabled;
        Vector3 origin;
        float initialTimer;
        Vector3 initialScale;

        public Particle(GameObject obj, Vector3 origin, Settings settings) {
            this.origin = origin;
            this.settings = settings;

            rt = obj.GetComponent<RectTransform>();
            image = rt.GetComponent<Image>();

            Disable();
        }

        public void Enable() {
            rt.gameObject.SetActive(true);
            rt.position = origin;
            disabled = false;

            timer = settings.GetRandomRange(settings.lifetimeMinMax);
            speed = settings.GetRandomRange(settings.speedMinMax);
            float scale = settings.GetRandomRange(settings.scaleMinMax);
            rt.localScale = new Vector3(scale, scale, 1f);
            
            rt.Rotate(new Vector3(0f, 0f, Random.Range(0f, Mathf.PI * 2f)));
            direction = Random.Range(0, Mathf.PI * 2f);

            initialTimer = timer;
            initialScale = rt.localScale;
        }

        void Disable() {
            rt.gameObject.SetActive(false);
            disabled = true;
        }

        public void Update() {
            if (disabled)
                return;

            float t = (initialTimer - timer) / initialTimer;

            rt.localPosition += new Vector3(Mathf.Cos(direction), Mathf.Sin(direction), 0f) * speed * settings.speedOverLifetime.Evaluate(t) * Time.unscaledDeltaTime;
            rt.localScale = initialScale * settings.scaleOverLifetime.Evaluate(t);

            timer -= Time.unscaledDeltaTime;
            if (timer <= 0f) {
                disabled = true;
                Disable();
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, settings.alphaOverLifetime.Evaluate(t));
        }
    }

    [System.Serializable]
    public class Collection
    {
        public GameObject template;
        public int amount = 10;
        [Space]
        public Particle.Settings settings;

        Particle[] particles;
        RectTransform rt;

        public void Init(RectTransform rt) {
            this.rt = rt;

            particles = new Particle[amount];
            for (int i = 0; i < amount; i++) {
                particles[i] = new Particle(Instantiate(template, rt.transform), rt.position, settings);
            }

            Destroy(template);
        }

        public void StartEmitting() {
            for (int i = 0; i < particles.Length; i++) {
                particles[i].Enable();
            }
        }

        public void Update() {
            for (int i = 0; i < particles.Length; i++) {
                particles[i].Update();
            }
        }
    }

    public Collection[] collections;

    private void Awake() {
        RectTransform rt = GetComponent<RectTransform>();

        for (int i = 0; i < collections.Length; i++) {
            collections[i].Init(rt);
        }
    }

    private void Update() {
        for (int i = 0; i < collections.Length; i++) {
            collections[i].Update();
        }
    }

    public void StartEmitting() {
        for (int i = 0; i < collections.Length; i++) {
            collections[i].StartEmitting();
        }
    }
}
