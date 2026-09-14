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

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

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
            rectTransform.anchoredPosition = originalPosition;
        }

        OnDragEnded?.Invoke(eventData);
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}