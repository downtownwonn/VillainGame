using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpeakerPickupUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("팝업 이미지 교체용")]
    public Image laptopPopupImage;   // LaptopScreenPanel 자신의 Image 컴포넌트 연결
    public Sprite noSpeakerVersion;  // 스피커 없는 버전 이미지

    [Header("효과음")]
    public AudioClip pickupSound;
    public AudioSource audioSource;

    [Header("아이템 획득 팝업")]
    public Sprite popupItemSprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        bool added = InventoryManager.Instance.AddItem(itemData);
        if (added)
        {
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            gameObject.SetActive(false);

            if (laptopPopupImage != null && noSpeakerVersion != null)
            {
                laptopPopupImage.sprite = noSpeakerVersion;
            }

            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(popupItemSprite);
            }
        }
    }
}