using UnityEngine;
using UnityEngine.UI;

public class StoreInventoryManager : MonoBehaviour
{
    public static StoreInventoryManager Instance;

    [Header("Inventory Item Icons")]
    public Image[] slotIcons;

    private void Awake()
    {
        Instance = this;
    }

    public bool AddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("전달된 ItemData가 null입니다.");
            return false;
        }

        // 1번 슬롯부터 확인
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image slotIcon = slotIcons[i];

            // 빈 슬롯인지 확인
            if (slotIcon.sprite == null)
            {
                // [수정] ItemSprite -> itemSprite (소문자 i)
                slotIcon.sprite = itemData.itemSprite;

                // 아이콘 보이기
                Color color = slotIcon.color;
                color.a = 1f;
                slotIcon.color = color;

                // 아이템 설명 팝업 연동
                ItemInfoData infoData = slotIcon.GetComponent<ItemInfoData>();
                if (infoData != null)
                {
                    infoData.itemData = itemData;
                }

                return true;
            }
        }

        // 인벤토리가 가득 찬 경우
        return false;
    }
}