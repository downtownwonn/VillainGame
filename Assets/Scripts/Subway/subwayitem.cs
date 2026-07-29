using UnityEngine;

public enum ItemType
{
    HandleLeft,
    HandleRight,
    Scissors,
    Sunglasses
}

public class SubwayItem : MonoBehaviour
{
    public string itemName;
    public Sprite inventorySprite;
    public ItemType itemType;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        SubwayInventoryManager.Instance.AddItem(inventorySprite, itemType);
        
        Debug.Log(itemName + " 획득");

        gameObject.SetActive(false);
    }
}