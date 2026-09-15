using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemSprite;

    public void PickUpItem()
    {
        bool itemAdded = StoreInventoryManager.Instance.AddItem(itemSprite);

        if (itemAdded)
        {
            // 팝업 표시 코드 추가
            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(itemSprite);
            }

            gameObject.SetActive(false);
        }
    }
}