using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemSprite;
    public AudioClip pickupSound; // 획득 소리 파일

    private void OnMouseDown()
    {
        PickUpItem();
    }

    public void PickUpItem()
    {
        bool itemAdded = StoreInventoryManager.Instance.AddItem(itemSprite);

        if (itemAdded)
        {
            // 오브젝트가 꺼져도 소리가 끊기지 않고 위치에서 재생됨
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(itemSprite);
            }

            gameObject.SetActive(false);
        }
    }
}