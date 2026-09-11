using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItemData",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("Item")]
    public Sprite itemSprite;

    public string itemName;

    [TextArea(3, 6)]
    public string description;
}