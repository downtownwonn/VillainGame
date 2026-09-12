using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [System.Serializable]
    public class FlickerStep
    {
        [Range(0f, 1f)]
        public float alpha = 1f;

        public float duration = 1f;
    }

    [Header("ºû ¿ÀºêÁ§Æ®")]
    [SerializeField] private SpriteRenderer[] lights;

    [Header("¹à±â º¯È­")]
    [SerializeField]
    private FlickerStep[] flickerPattern =
    {
        new FlickerStep { alpha = 1.0f, duration = 1.0f },
        new FlickerStep { alpha = 0.6f, duration = 0.5f },
        new FlickerStep { alpha = 0.5f, duration = 0.5f },
        new FlickerStep { alpha = 0.8f, duration = 1.0f }
    };

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            foreach (FlickerStep step in flickerPattern)
            {
                SetAlpha(step.alpha);

                yield return new WaitForSeconds(step.duration);
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer light in lights)
        {
            if (light == null)
                continue;

            Color color = light.color;
            color.a = alpha;
            light.color = color;
        }
    }
}