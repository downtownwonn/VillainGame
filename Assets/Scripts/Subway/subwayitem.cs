using UnityEngine;

public class SubwayItem : MonoBehaviour
{
    public string itemName;
    public Sprite inventorySprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        SubwayInventoryManager.Instance.AddItem(inventorySprite);
        
        Debug.Log(itemName + " 획득");

        gameObject.SetActive(false);
    }
}