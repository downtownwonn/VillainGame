using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas canvas;

    [SerializeField] private bool keepVisualSizeWhileDragging;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalSizeDelta;
    private Vector2 dragVisualSize;

    [HideInInspector]
    public bool wasDropped = false;

    public Action<PointerEventData> OnDragEnded;

    public CinemaItemType ItemType
    {
        get
        {
            CinemaInventoryItemData data =
                GetComponent<CinemaInventoryItemData>();

            if (data == null)
                return CinemaItemType.None;

            return data.itemType;
        }
    }

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

        if (keepVisualSizeWhileDragging)
        {
            originalAnchorMin = rectTransform.anchorMin;
            originalAnchorMax = rectTransform.anchorMax;
            originalSizeDelta = rectTransform.sizeDelta;
            dragVisualSize = rectTransform.rect.size;
        }

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        if (keepVisualSizeWhileDragging)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = dragVisualSize;
        }

        canvasGroup.blocksRaycasts = false;

        Debug.Log("드래그 아이템 = " + ItemType);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag 호출됨. wasDropped = " + wasDropped);

        canvasGroup.blocksRaycasts = true;

        if (!wasDropped)
        {
            transform.SetParent(originalParent);
            RestoreOriginalLayout();
            rectTransform.anchoredPosition = originalPosition;
        }

        OnDragEnded?.Invoke(eventData);
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        RestoreOriginalLayout();
        rectTransform.anchoredPosition = originalPosition;
    }

    private void RestoreOriginalLayout()
    {
        if (!keepVisualSizeWhileDragging)
            return;

        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.sizeDelta = originalSizeDelta;
    }
}
