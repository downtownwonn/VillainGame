using UnityEngine;

public class HighlightPulse : MonoBehaviour
{
    private SpriteRenderer sr;

    public float speed = 2f;      // 깜빡이는 속도
    public float minAlpha = 0.1f; // 가장 흐려질 때 투명도
    public float maxAlpha = 0.2f; // 가장 진해질 때 투명도

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}