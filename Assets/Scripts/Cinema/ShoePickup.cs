using UnityEngine;
using UnityEngine.EventSystems;

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

    [Header("Popup Blocking")]
    [SerializeField] private GameObject popupToBlock;

    [Header("Settings")]




    public bool disableObject = true;

    public static bool IsShoePickedUp = false;

    private bool pickedUp = false;


    private void OnMouseDown()
    {
        // 팝업이 떠 있는 동안에는 신발 클릭 방지
        if (popupToBlock != null && popupToBlock.activeInHierarchy)
        {
            return;
        }

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