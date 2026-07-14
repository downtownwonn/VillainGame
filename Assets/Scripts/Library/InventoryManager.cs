using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Image[] slotIcons;
    private int currentSlotIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(Sprite itemSprite)
    {
        if (currentSlotIndex >= slotIcons.Length)
        {
            Debug.Log("인벤토리가 가득 찼어요!");
            return false;
        }

        Image slot = slotIcons[currentSlotIndex];
        slot.sprite = itemSprite;

        Color c = slot.color;
        c.a = 1f;
        slot.color = c;

        currentSlotIndex++;
        return true;
    }
}