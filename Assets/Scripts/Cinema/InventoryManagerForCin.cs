using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerForCin : MonoBehaviour
{
    public static InventoryManagerForCin Instance;

    [Header("Inventory Slot Icons")]
    public Image[] slotIcons;

    // 슬롯 1은 영화 티켓이 사용하므로
    // 아이템은 슬롯 2부터 들어간다.
    private int currentSlotIndex = 1;

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            Debug.LogError("AddItem: itemSprite가 비어 있습니다!");
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

        // 아이템 이미지 설정
        slot.sprite = itemSprite;

        // 이미지 보이기
        Color c = slot.color;
        c.a = 1f;
        slot.color = c;

        // 다음 아이템은 다음 슬롯
        currentSlotIndex++;

        return true;
    }
}