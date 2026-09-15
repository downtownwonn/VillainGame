using UnityEngine;
using UnityEngine.UI;

public class SubwayInventoryManager : MonoBehaviour
{
    public static SubwayInventoryManager Instance;

    public Image[] itemIcons;
    public Sprite scissorsSprite;

    private void Awake()
    {
        Instance = this;

        foreach (Image img in itemIcons)
        {
            ClearSlot(img);
        }
    }

    // =========================================================
    // 아이템 획득
    // =========================================================

    public void AddItem(ItemData itemData, ItemType itemType)
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData가 없습니다.");
            return;
        }

        for (int i = 0; i < itemIcons.Length; i++)
        {
            Image slot = itemIcons[i];

            if (!slot.enabled || slot.sprite == null)
            {
                slot.sprite = itemData.itemSprite;
                slot.preserveAspect = true;
                slot.enabled = true;

                // 기존 ItemType 기능 유지
                InventoryItemData inventoryData =
                    slot.GetComponent<InventoryItemData>();

                if (inventoryData != null)
                {
                    inventoryData.itemType = itemType;

                    Debug.Log(
                        "인벤토리 슬롯 " + i +
                        "에 들어간 아이템 타입 = " +
                        inventoryData.itemType
                    );
                }
                else
                {
                    Debug.LogError(
                        "인벤토리 슬롯 " + i +
                        "에 InventoryItemData가 없습니다!"
                    );
                }

                // ItemInfoPopup용 ItemData 연결
                ItemInfoData infoData =
                    slot.GetComponent<ItemInfoData>();

                if (infoData != null)
                {
                    infoData.itemData = itemData;
                }
                else
                {
                    Debug.LogWarning(
                        "인벤토리 슬롯 " + i +
                        "에 ItemInfoData가 없습니다!"
                    );
                }

                return;
            }
        }

        Debug.LogWarning("인벤토리에 빈 슬롯이 없습니다.");
    }

    // =========================================================
    // 슬롯 완전히 비우기
    // =========================================================

    public void ClearSlot(Image slot)
    {
        if (slot == null)
            return;

        slot.sprite = null;
        slot.enabled = false;

        InventoryItemData inventoryData =
            slot.GetComponent<InventoryItemData>();

        if (inventoryData != null)
        {
            inventoryData.itemType = default;
        }

        ItemInfoData infoData =
            slot.GetComponent<ItemInfoData>();

        if (infoData != null)
        {
            infoData.itemData = null;
        }
    }

    // =========================================================
    // 아이템 합치기
    // =========================================================

    public bool TryCombine(
        Image first,
        Image second,
        ItemData scissorsItemData
    )
    {
        InventoryItemData firstData =
            first.GetComponent<InventoryItemData>();

        InventoryItemData secondData =
            second.GetComponent<InventoryItemData>();

        if (firstData == null || secondData == null)
        {
            Debug.LogError("InventoryItemData가 없습니다.");
            return false;
        }

        bool canCombine =
            (firstData.itemType == ItemType.HandleLeft &&
             secondData.itemType == ItemType.HandleRight)
            ||
            (firstData.itemType == ItemType.HandleRight &&
             secondData.itemType == ItemType.HandleLeft);

        if (!canCombine)
        {
            Debug.Log("합칠 수 없는 아이템입니다.");
            return false;
        }

        // ---------------------------------------------------------
        // 두 번째 슬롯을 가위로 변경
        // ---------------------------------------------------------

        second.sprite = scissorsSprite;
        second.preserveAspect = true;
        second.enabled = true;

        secondData.itemType = ItemType.Scissors;

        // ---------------------------------------------------------
        // 가위 ItemData 연결
        // ---------------------------------------------------------

        ItemInfoData secondInfoData =
            second.GetComponent<ItemInfoData>();

        if (secondInfoData != null)
        {
            secondInfoData.itemData = scissorsItemData;
        }
        else
        {
            Debug.LogWarning(
                "가위 슬롯에 ItemInfoData가 없습니다."
            );
        }

        // ---------------------------------------------------------
        // 첫 번째 슬롯 비우기
        // ---------------------------------------------------------

        ClearSlot(first);

        // ---------------------------------------------------------
        // 드래그한 아이콘의 위치 복구
        // ---------------------------------------------------------

        SubwayDraggableItem draggable =
            first.GetComponent<SubwayDraggableItem>();

        if (draggable != null)
        {
            draggable.ReturnToOriginalPosition();
        }

        return true;
    }
}