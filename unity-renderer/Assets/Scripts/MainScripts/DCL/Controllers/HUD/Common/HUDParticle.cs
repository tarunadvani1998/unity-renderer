using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDParticle : MonoBehaviour
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

    RectTransform rt;
    float timer, direction, speed;

    Settings settings;
    Image image;
    float initialTimer;
    Vector3 initialScale;

    public void Initialize(Settings settings, RectTransform originRT) {
        this.settings = settings;

        rt = GetComponent<RectTransform>();
        rt.position = GetRandomPositionInRectTransform(originRT);
        image = rt.GetComponent<Image>();

        gameObject.SetActive(true);

        timer = settings.GetRandomRange(settings.lifetimeMinMax);
        speed = settings.GetRandomRange(settings.speedMinMax);
        float scale = settings.GetRandomRange(settings.scaleMinMax);
        rt.localScale = new Vector3(scale, scale, 1f);

        rt.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        direction = Random.Range(0, Mathf.PI * 2f);

        initialTimer = timer;
        initialScale = rt.localScale;
    }

    public void Update() {
        float t = (initialTimer - timer) / initialTimer;

        rt.localPosition += new Vector3(Mathf.Cos(direction), Mathf.Sin(direction), 0f) * speed * settings.speedOverLifetime.Evaluate(t) * Time.unscaledDeltaTime;
        rt.localScale = initialScale * settings.scaleOverLifetime.Evaluate(t);

        timer -= Time.unscaledDeltaTime;
        if (timer <= 0f) {
            Discard();
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, settings.alphaOverLifetime.Evaluate(t));
    }

    void Discard() {
        Destroy(gameObject);
    }

    Vector3 GetRandomPositionInRectTransform(RectTransform rt) {
        Vector3 r = new Vector3 {
            x = rt.position.x + Random.Range(0f, rt.rect.width) - (rt.rect.width * rt.pivot.x),
            y = rt.position.y + Random.Range(0f, rt.rect.height) - (rt.rect.height * rt.pivot.y)
        };
        return r;
    }
}
