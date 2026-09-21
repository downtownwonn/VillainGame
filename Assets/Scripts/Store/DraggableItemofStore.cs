using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 1. IPointerClickHandler 추가
public class DraggableItemofStore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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

        // ★ 핵심: Canvas 밖의 월드 오브젝트(Box/Polygon Collider 2D 등) 감지
        Vector2 itemWorldPos = Camera.main.ScreenToWorldPoint(rectTransform.position);
        Collider2D hitCollider = Physics2D.OverlapPoint(itemWorldPos);

        if (hitCollider != null)
        {
            // 대상 오브젝트에 부착된 어떤 컴포넌트든 상관없이 드롭 이벤트를 전달할 수 있게 SendMessage 활용
            // (받는 쪽에서 OnItemDropped(this) 혹은 알아서 처리 가능)
            hitCollider.SendMessage("OnItemDropped", this, SendMessageOptions.DontRequireReceiver);
        }

        // wasDropped가 true가 되지 않았다면 원래 위치로 복귀
        
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();

        if (img != null && img.sprite != null)
        {
            // 스프라이트 이름에 "controller"가 포함되어 있는지 확인 (대소문자 구분 없음)
            if (img.sprite.name.IndexOf("controller", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ItemPopupManager.Instance.ShowPopup(img.sprite);
            }
        }
    }
}