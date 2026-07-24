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


    public bool AddItem(Sprite itemSprite)
    {
        // 1번 슬롯부터 확인
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image slotIcon = slotIcons[i];


            // 빈 슬롯인지 확인
            if (slotIcon.sprite == null)
            {
                // 아이템 아이콘 넣기
                slotIcon.sprite = itemSprite;


                // 아이콘 보이기
                Color color = slotIcon.color;
                color.a = 1f;
                slotIcon.color = color;


                return true;
            }
        }


        // 인벤토리가 가득 찬 경우
        return false;
    }
}