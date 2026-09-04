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

    public bool AddItem(Sprite itemSprite)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image slot = slotIcons[i];

            if (slot.sprite == null) // 비어있는 슬롯 판정
            {
                slot.sprite = itemSprite;

                Color c = slot.color;
                c.a = 1f;
                slot.color = c;

                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼어요!");
        return false;
    }
}