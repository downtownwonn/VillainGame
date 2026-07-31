using UnityEngine;

public class ShoePickup : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject inventoryShoe;     // 인벤토리에 들어갈 신발 아이콘

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;        // 신발 획득 효과음

    [Header("Settings")]
    public bool disableObject = true;    // 클릭한 신발 오브젝트 숨길지 여부

    private bool pickedUp = false;

    private void OnMouseDown()
    {
        if (pickedUp)
            return;

        pickedUp = true;

        // 신발 획득 효과음
        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        // 인벤토리 신발 활성화
        if (inventoryShoe != null)
            inventoryShoe.SetActive(true);

        // 빌런 신발 숨기기
        if (disableObject)
            gameObject.SetActive(false);
    }
}