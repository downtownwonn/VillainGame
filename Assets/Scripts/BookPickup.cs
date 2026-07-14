using UnityEngine;

public class BookPickup : MonoBehaviour
{
    public Sprite bookIcon;

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(bookIcon);
        if (added)
        {
            gameObject.SetActive(false);
        }
    }
}