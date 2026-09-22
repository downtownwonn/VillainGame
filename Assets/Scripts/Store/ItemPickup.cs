using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Sound")]
    public AudioClip pickupSound;

    private void OnMouseDown()
    {
        PickUpItem();
    }

    public void PickUpItem()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"{gameObject.name}에 ItemData가 연결되지 않았습니다!");
            return;
        }

        bool itemAdded = StoreInventoryManager.Instance.AddItem(itemData);

        if (itemAdded)
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (ItemGetPopup.Instance != null)
            {
                // [수정] ItemSprite -> itemSprite (소문자 i)
                ItemGetPopup.Instance.Show(itemData.itemSprite);
            }

            gameObject.SetActive(false);
        }
    }
}