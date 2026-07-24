using UnityEngine;
using UnityEngine.EventSystems;

public class DragScaleEffect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private float dragScale = 1.5f;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.localScale = originalScale * dragScale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}