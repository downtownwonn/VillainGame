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

    [Header("Settings")]
    public bool disableObject = true;

    public static bool IsShoePickedUp = false;

    private bool pickedUp = false;

    private void OnMouseDown()
    {
        // UI를 클릭하고 있다면 신발을 클릭하지 않음
        // (팝업 버튼, 팝업 배경 등을 클릭했을 때 신발이 먹히는 것을 방지)
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
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