using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerForCin : MonoBehaviour
{
    public static InventoryManagerForCin Instance;

    [Header("Inventory Slot Icons")]
    public Image[] slotIcons;

    private int currentSlotIndex = 1;

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(ItemData itemData, CinemaItemType itemType)
    {
        if (itemData == null)
        {
            Debug.LogError("AddItem: ItemData가 비어 있습니다!");
            return false;
        }

        if (itemData.itemSprite == null)
        {
            Debug.LogError("AddItem: ItemData의 itemSprite가 비어 있습니다!");
            return false;
        }

        if (slotIcons == null || slotIcons.Length == 0)
        {
            Debug.LogError("AddItem: slotIcons가 연결되지 않았습니다!");
            return false;
        }

        if (currentSlotIndex >= slotIcons.Length)
        {
            Debug.Log("인벤토리가 가득 찼어요!");
            return false;
        }

        Image slot = slotIcons[currentSlotIndex];

        if (slot == null)
        {
            Debug.LogError("AddItem: 해당 슬롯의 Image가 연결되지 않았습니다!");
            return false;
        }

        // 아이템 이미지 넣기
        slot.sprite = itemData.itemSprite;
        slot.preserveAspect = true;
        slot.enabled = true;

        Color c = slot.color;
        c.a = 1f;
        slot.color = c;

        // 기존 CinemaItemType 저장
        CinemaInventoryItemData cinemaData =
            slot.GetComponent<CinemaInventoryItemData>();

        if (cinemaData != null)
        {
            cinemaData.itemType = itemType;
        }
        else
        {
            Debug.LogError(
                "해당 ItemIcon에 CinemaInventoryItemData가 없습니다!"
            );
        }

        // 아이템 설명 정보 저장
        ItemInfoData infoData =
            slot.GetComponent<ItemInfoData>();

        if (infoData != null)
        {
            infoData.itemData = itemData;
        }
        else
        {
            Debug.LogError(
                "해당 ItemIcon에 ItemInfoData가 없습니다!"
            );
        }

        currentSlotIndex++;

        return true;
    }
}