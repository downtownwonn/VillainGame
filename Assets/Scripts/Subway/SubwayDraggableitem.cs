using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubwayDraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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

        if (img.sprite == null)
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
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // =====================================================
        // 1. 먼저 기존의 인벤토리 아이템 합치기 확인
        // =====================================================

        Image targetImage = null;

        foreach (GameObject obj in eventData.hovered)
        {
            Image image = obj.GetComponent<Image>();

            if (image == null)
                continue;

            if (image == GetComponent<Image>())
                continue;

            if (image.sprite == null)
                continue;

            InventoryItemData data =
                image.GetComponent<InventoryItemData>();

            if (data != null)
            {
                targetImage = image;
                break;
            }
        }

        // 합치기
        if (targetImage != null)
        {
            bool combined =
                SubwayInventoryManager.Instance.TryCombine(
                    GetComponent<Image>(),
                    targetImage
                );

            if (combined)
            {
                return;
            }
        }


        // =====================================================
        // 2. 백팩 빌런 확인
        // =====================================================

        Vector3 screenPosition = eventData.position;

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(screenPosition);

        worldPosition.z = 0f;

        Collider2D hitCollider =
            Physics2D.OverlapPoint(worldPosition);

        if (hitCollider != null)
        {
            BackpackVillainTarget backpack =
                hitCollider.GetComponent<BackpackVillainTarget>();

            if (backpack != null)
            {
                bool used =
                    backpack.UseScissors(
                        GetComponent<Image>()
                    );

                if (used)
                {
                    Debug.Log("백팩에 가위를 사용했습니다.");

                    return;
                }
            }
        }


        // =====================================================
        // 3. 아무것도 안 했으면 원래 자리로 복귀
        // =====================================================

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;

        OnDragEnded?.Invoke(eventData);
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();

        if (img.sprite != null)
        {
            ItemPopupManager.Instance.ShowPopup(img.sprite);
        }
    }
}