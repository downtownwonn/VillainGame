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


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvas = GetComponentInParent<Canvas>();
    }


    // =========================================================
    // 드래그 시작
    // =========================================================

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


    // =========================================================
    // 드래그 중
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }


    // =========================================================
    // 드래그 종료
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        InventoryItemData draggedItemData =
            GetComponent<InventoryItemData>();
// =====================================================
// R3
// 경호원 3인조 → 자르반 할아버지
// =====================================================

if (draggedItemData != null &&
    draggedItemData.itemType == ItemType.Bodyguards)
{
    Vector3 worldPosition =
        Camera.main.ScreenToWorldPoint(eventData.position);

    worldPosition.z = 0f;

    Collider2D hit =
        Physics2D.OverlapPoint(worldPosition);

    if (hit != null)
    {
        JarvanVillain villain =
            hit.GetComponent<JarvanVillain>();

        if (villain == null)
        {
            villain =
                hit.GetComponentInParent<JarvanVillain>();
        }

        if (villain != null)
        {
            Image bodyguardImage =
                GetComponent<Image>();

            bool used =
                villain.UseBodyguards(bodyguardImage);

            if (used)
            {
                ReturnToOriginalPosition();
                return;
            }
        }
    }
}
// =====================================================
// R3
// 선글라스 → 직장인
// =====================================================

if (draggedItemData != null &&
    draggedItemData.itemType == ItemType.Sunglasses)
{
    Vector3 worldPosition =
        Camera.main.ScreenToWorldPoint(eventData.position);

    worldPosition.z = 0f;

    Collider2D[] hits =
        Physics2D.OverlapPointAll(worldPosition);

    foreach (Collider2D hit in hits)
    {
        BodyguardCharacter bodyguard =
            hit.GetComponent<BodyguardCharacter>();

        if (bodyguard == null)
        {
            bodyguard =
                hit.GetComponentInParent<BodyguardCharacter>();
        }

        if (bodyguard != null)
        {
            Debug.Log("선글라스를 직장인에게 사용!");

            // 선글라스 착용
            bodyguard.PutOnSunglasses();

            // 사용한 선글라스 제거
            Image sunglassesImage =
                GetComponent<Image>();

            if (sunglassesImage != null)
            {
                sunglassesImage.sprite = null;
                sunglassesImage.enabled = false;
            }

            ReturnToOriginalPosition();

            return;
        }
    }
}
        // =====================================================
        // R2
        // 상모 → 장구 부원
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Sangmo)
        {
            foreach (GameObject obj in eventData.hovered)
            {
                JangguCharacter jangguCharacter =
                    obj.GetComponent<JangguCharacter>();

                if (jangguCharacter != null)
                {
                    jangguCharacter.ReceiveSangmo();

                    Image usedItemImage = GetComponent<Image>();

                    if (usedItemImage != null)
                    {
                        usedItemImage.sprite = null;
                        usedItemImage.enabled = false;
                    }

                    ReturnToOriginalPosition();

                    return;
                }
            }
        }


        // =====================================================
        // R2
        // 장구 → 단소 살인마
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Janggu)
        {
            foreach (GameObject obj in eventData.hovered)
            {
                DansoVillain villain =
                    obj.GetComponent<DansoVillain>();

                if (villain != null)
                {
                    villain.ReceiveJanggu();

                    Image usedItemImage = GetComponent<Image>();

                    if (usedItemImage != null)
                    {
                        usedItemImage.sprite = null;
                        usedItemImage.enabled = false;
                    }

                    ReturnToOriginalPosition();

                    return;
                }
            }
        }


        // =====================================================
        // R1
        // 가위 → 백팩 빌런
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Scissors)
        {
            Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(eventData.position);

            worldPosition.z = 0f;

            Collider2D hit =
                Physics2D.OverlapPoint(worldPosition);

            if (hit != null)
            {
                BackpackVillainTarget backpack =
                    hit.GetComponent<BackpackVillainTarget>();

                if (backpack == null)
                {
                    backpack =
                        hit.GetComponentInParent<BackpackVillainTarget>();
                }

                if (backpack != null)
                {
                    Image scissorsImage =
                        GetComponent<Image>();

                    bool used =
                        backpack.UseScissors(scissorsImage);

                    if (used)
                    {
                        Debug.Log("가위 → 백팩 빌런 성공!");

                        ReturnToOriginalPosition();

                        return;
                    }
                }
            }
        }


        // =====================================================
        // R1
        // 인벤토리 아이템끼리 합치기
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


        // =====================================================
        // R1
        // 손잡이 + 손잡이 = 가위
        // =====================================================

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
        // 아무것도 성공하지 않았으면 원래 자리로
        // =====================================================

        ReturnToOriginalPosition();

        if (OnDragEnded != null)
        {
            OnDragEnded.Invoke(eventData);
        }
    }

    // =========================================================
    // 원래 인벤토리 슬롯으로 복귀
    // =========================================================

    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }

        rectTransform.anchoredPosition = originalPosition;
    }


    // =========================================================
    // 인벤토리 아이템 클릭
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
{
    Image img = GetComponent<Image>();

    // 빈 슬롯이면 아무 반응 없음
    if (img == null || img.sprite == null)
        return;

    InventoryItemData itemData =
        GetComponent<InventoryItemData>();

    // 아이템 데이터가 없으면 아무 반응 없음
    if (itemData == null)
        return;

    if (SubwayItemPopupManager.Instance != null)
    {
        SubwayItemPopupManager.Instance.ShowPopup(
            img.sprite,
            itemData.itemType
        );
    }
}
}