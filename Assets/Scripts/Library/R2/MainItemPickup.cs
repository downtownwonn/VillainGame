using UnityEngine;

public class MainItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    public SpriteRenderer backgroundRenderer;
    public Sprite newBackgroundSprite;

    [Header("아이템 획득 팝업")]
    public Sprite popupItemSprite;

    [Header("효과음")]
    public AudioClip pickupSound;   // 인스펙터에서 효과음 파일 연결
    public AudioSource audioSource; // AudioManager의 AudioSource 연결

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(itemData);
        if (added)
        {
            // 효과음 재생
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }

            gameObject.SetActive(false);

            // 아이템 획득 팝업 표시
            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(popupItemSprite);
            }
        }
    }
}