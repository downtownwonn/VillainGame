using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemSprite;


    public void PickUpItem()
    {
        bool itemAdded =
            StoreInventoryManager.Instance.AddItem(itemSprite);


        if (itemAdded)
        {
            gameObject.SetActive(false);
        }
    }
}