using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemofStore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas canvas;

    [Header("드래그 크기 설정")]
    [Tooltip("드래그하는 동안 몇 배로 키울지 (1 = 그대로, 1.3 = 30% 크게)")]
    [SerializeField] private float dragScale = 1.3f;
    [Tooltip("칸에서 빠져나올 때 크기가 찌그러지지 않게 유지")]
    [SerializeField] private bool keepVisualSizeWhileDragging = true;

    private Vector3 originalScale;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalSizeDelta;
    private bool isDragging = false;

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
        originalScale = rectTransform.localScale;
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
        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // 칸 안에서 보이던 실제 크기 저장
        Vector2 visualSize = rectTransform.rect.size;
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalSizeDelta = rectTransform.sizeDelta;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        if (keepVisualSizeWhileDragging)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = visualSize;
        }

        // 드래그 중 크게
        rectTransform.localScale = originalScale * dragScale;

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

        RestoreOriginalLayout();
        rectTransform.anchoredPosition = originalPosition;
    }

    private void RestoreOriginalLayout()
    {
        // 크기 원래대로
        rectTransform.localScale = originalScale;

        if (keepVisualSizeWhileDragging && isDragging)
        {
            rectTransform.anchorMin = originalAnchorMin;
            rectTransform.anchorMax = originalAnchorMax;
            rectTransform.sizeDelta = originalSizeDelta;
        }

        isDragging = false;
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
}