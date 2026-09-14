using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Image[] slotIcons;

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(ItemData itemData)
    {
        if (itemData == null || itemData.itemSprite == null)
        {
            Debug.LogWarning("AddItem: itemData 또는 itemSprite가 비어있습니다.");
            return false;
        }

        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image slot = slotIcons[i];

            if (slot.sprite == null) // 비어있는 슬롯 판정
            {
                slot.sprite = itemData.itemSprite;

                Color c = slot.color;
                c.a = 1f;
                slot.color = c;

                // 설명 팝업 연동: 이 슬롯(ItemIcon)의 ItemInfoData에 itemData 채워주기
                ItemInfoData infoData = slot.GetComponent<ItemInfoData>();
                if (infoData != null)
                {
                    infoData.itemData = itemData;
                }

                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼어요!");
        return false;
    }
}