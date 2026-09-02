using UnityEngine;

public class MainItemPickup : MonoBehaviour
{
    public Sprite itemIcon;
    public SpriteRenderer backgroundRenderer;
    public Sprite newBackgroundSprite;

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(itemIcon);
        if (added)
        {
            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }

            gameObject.SetActive(false);
        }
    }
}