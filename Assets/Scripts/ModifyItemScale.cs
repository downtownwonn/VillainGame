using UnityEngine;
using UnityEngine.EventSystems;

public class DragScaleEffect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private float dragScale = 1.5f;

    [SerializeField]
    private Vector2 dragScaleXY = Vector2.one;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.localScale = new Vector3(
            originalScale.x * dragScale * dragScaleXY.x,
            originalScale.y * dragScale * dragScaleXY.y,
            originalScale.z * dragScale);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}
