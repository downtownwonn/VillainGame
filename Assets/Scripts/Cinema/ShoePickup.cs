using UnityEngine;
using UnityEngine.UI;

public class ShoePickup : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject inventoryShoe;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    [Header("Settings")]
    public bool disableObject = true;

    // 신발을 얻었는지 다른 스크립트에서 확인할 수 있게 함
    public static bool IsShoePickedUp = false;

    private bool pickedUp = false;

    private void OnMouseDown()
    {
        if (pickedUp)
            return;

        pickedUp = true;

        // 신발 획득
        IsShoePickedUp = true;

        // 신발 획득 효과음
        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        // 인벤토리에 신발 추가
        if (inventoryShoe != null)
        {
            Image shoeImage = inventoryShoe.GetComponent<Image>();

            if (shoeImage != null)
            {
                InventoryManagerForCin.Instance.AddItem(shoeImage.sprite);
            }
        }

        // 기존 인벤토리 아이콘 숨기기
        if (inventoryShoe != null)
            inventoryShoe.SetActive(false);

        // 화면의 빌런 신발 숨기기
        if (disableObject)
            gameObject.SetActive(false);
    }
}