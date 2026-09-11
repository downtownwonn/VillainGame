using UnityEngine;

public class ShoePickup : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Sprite shoeSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Popup")]
    [SerializeField] private Sprite popupItemSprite;

    [Header("Settings")]
    public bool disableObject = true;

    public static bool IsShoePickedUp = false;

    private bool pickedUp = false;

    private void OnMouseDown()
    {
        if (pickedUp)
            return;

        pickedUp = true;
        IsShoePickedUp = true;

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        if (itemData != null)
        {
            InventoryManagerForCin.Instance.AddItem(
                itemData,
                CinemaItemType.Shoe
            );
        }
        else
        {
            Debug.LogError("ShoePickup: itemData가 비어 있습니다!");
        }

        if (ItemGetPopup.Instance != null && popupItemSprite != null)
        {
            ItemGetPopup.Instance.Show(popupItemSprite);
        }

        if (disableObject)
            gameObject.SetActive(false);
    }
}