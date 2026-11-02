using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Gives buttons a small, animated scale response on pointer hover.</summary>
[DisallowMultipleComponent]
public sealed class MenuHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Range(1f, 1.2f)] private float hoverScale = 1.06f;
    [SerializeField, Min(1f)] private float responseSpeed = 18f;

    private RectTransform target;
    private Vector3 baseScale = Vector3.one;
    private float targetScale = 1f;

    public void SetTarget(RectTransform rectTransform)
    {
        target = rectTransform != null ? rectTransform : transform as RectTransform;
        baseScale = target != null ? target.localScale : Vector3.one;
    }

    public void SetHoverScale(float scale)
    {
        hoverScale = Mathf.Clamp(scale, 1f, 1.2f);
    }

    private void Awake()
    {
        if (target == null)
        {
            SetTarget(transform as RectTransform);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = 1f;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        float scale = Mathf.Lerp(target.localScale.x / baseScale.x, targetScale, 1f - Mathf.Exp(-responseSpeed * Time.unscaledDeltaTime));
        target.localScale = baseScale * scale;
    }

    private void OnDisable()
    {
        targetScale = 1f;
        if (target != null)
        {
            target.localScale = baseScale;
        }
    }
}
