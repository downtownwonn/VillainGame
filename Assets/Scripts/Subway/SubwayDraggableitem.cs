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

        // 아이템이 없는 슬롯이면 드래그하지 않음
        if (img == null || img.sprite == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        wasDropped = false;

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // 드래그 중에는 Canvas의 가장 위로 올림
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // 드래그 중에는 다른 UI의 Raycast를 방해하지 않음
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


        // =========================================================
        // R2
        // 상모를 장구 부원에게 드래그했는지 확인
        // =========================================================

        InventoryItemData draggedItemData =
            GetComponent<InventoryItemData>();

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Sangmo)
        {
            foreach (GameObject obj in eventData.hovered)
            {
                JangguCharacter jangguCharacter =
                    obj.GetComponent<JangguCharacter>();

                if (jangguCharacter != null)
                {
                    // 장구 부원이 춤추기 시작
                    jangguCharacter.ReceiveSangmo();

                    // 사용한 상모 아이템 제거
                    gameObject.SetActive(false);

                    return;
                }
            }
        }


        // =========================================================
        // 기존 R1
        // 인벤토리 아이템끼리 합치기
        // =========================================================

        Image targetImage = null;

        foreach (GameObject obj in eventData.hovered)
        {
            Image image = obj.GetComponent<Image>();

            if (image == null)
                continue;

            // 자기 자신이면 무시
            if (image == GetComponent<Image>())
                continue;

            // 아이템이 없는 슬롯이면 무시
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


        // =========================================================
        // 아이템 합치기
        // =========================================================

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


        // =========================================================
        // 아무것도 성공하지 못했으면 원래 자리로 돌아감
        // =========================================================

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

        if (img != null && img.sprite != null)
        {
            ItemPopupManager.Instance.ShowPopup(img.sprite);
        }
    }
}