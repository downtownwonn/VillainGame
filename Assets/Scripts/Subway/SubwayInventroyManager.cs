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

        // 시작할 때 아이콘 숨기기
        foreach (Image img in itemIcons)
        {
            img.sprite = null;
            img.enabled = false;
        }
    }

    public void AddItem(Sprite sprite, ItemType itemType)
{
    for (int i = 0; i < itemIcons.Length; i++)
    {
        if (!itemIcons[i].enabled)
        {
            itemIcons[i].sprite = sprite;
            itemIcons[i].preserveAspect = true;
            itemIcons[i].enabled = true;

            InventoryItemData data = itemIcons[i].GetComponent<InventoryItemData>();

            if (data != null)
            {
                data.itemType = itemType;
            }

            return;
        }
    }
}
}