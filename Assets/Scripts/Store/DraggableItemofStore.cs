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

        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalSizeDelta = rectTransform.sizeDelta;

        Vector2 dragPosition = rectTransform.anchoredPosition;
        Vector2 dragSize = rectTransform.rect.size;
        RectTransform canvasRect = canvas != null ? canvas.transform as RectTransform : null;

        // 인벤토리 부모와 캔버스 사이의 배율 차이를 반영해 화면에 보이는 크기를 캔버스 좌표로 변환합니다.
        if (keepVisualSizeWhileDragging && canvasRect != null)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            bool converted = true;

            for (int i = 0; i < worldCorners.Length; i++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCorners[i]);
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect, screenPoint, uiCamera, out Vector2 canvasPoint))
                {
                    converted = false;
                    break;
                }

                min = Vector2.Min(min, canvasPoint);
                max = Vector2.Max(max, canvasPoint);
            }

            if (converted)
            {
                Vector3 worldCenter = rectTransform.position;
                Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCenter);
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect, screenCenter, uiCamera, out Vector2 canvasCenter))
                {
                    dragPosition = canvasCenter;
                    dragSize = max - min;
                }
            }
        }

        transform.SetParent(canvas != null ? canvas.transform : originalParent, false);
        transform.SetAsLastSibling();

        if (keepVisualSizeWhileDragging && canvasRect != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = dragSize;
            rectTransform.localScale = Vector3.one * dragScale;
            rectTransform.anchoredPosition = dragPosition;
        }
        else
        {
            rectTransform.localScale = originalScale * dragScale;
        }

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
