using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 1. IPointerClickHandler 제거
public class DraggableItemofStore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas canvas;

    [HideInInspector]
    public bool wasDropped = false;

    public Action<PointerEventData> OnDragEnded;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();

        if (img == null || img.sprite == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        wasDropped = false;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag 호출됨. wasDropped = " + wasDropped);

        canvasGroup.blocksRaycasts = true;

        Vector2 itemWorldPos = Camera.main.ScreenToWorldPoint(rectTransform.position);
        Collider2D hitCollider = Physics2D.OverlapPoint(itemWorldPos);

        if (hitCollider != null)
        {
            hitCollider.SendMessage("OnItemDropped", this, SendMessageOptions.DontRequireReceiver);
        }

        ReturnToOriginalPosition();

        OnDragEnded?.Invoke(eventData);
    }

    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    public void ClearItem()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.sprite = null;
            img.color = new Color(1f, 1f, 1f, 0f);
        }

        ReturnToOriginalPosition();

        wasDropped = false;
        canvasGroup.blocksRaycasts = true;
    }

    // 2. OnPointerClick 메서드 전체 삭제 완료
}