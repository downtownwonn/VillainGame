using UnityEngine;

public class BookPickup : MonoBehaviour
{
    public Sprite bookIcon;
    private SpriteRenderer sr;

    [Header("팝업 이미지 교체용")]
    public UnityEngine.UI.Image laptopPopupImage;
    public Sprite noSpeakerVersion;

    [Header("효과음")]
    public AudioClip pickupSound;   // 인스펙터에서 효과음 파일 연결
    public AudioSource audioSource; // AudioManager의 AudioSource 연결

    [Header("아이템 획득 팝업")]
    public Sprite popupItemSprite;  // 획득 팝업창에 보여줄 이미지

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(bookIcon);
        if (added)
        {
            // 효과음 재생
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            gameObject.SetActive(false);

            if (laptopPopupImage != null && noSpeakerVersion != null)
            {
                laptopPopupImage.sprite = noSpeakerVersion;
            }

            // 아이템 획득 팝업 표시
            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(popupItemSprite);
            }
        }
    }

    void OnMouseEnter()
    {
        Color c = sr.color;
        c.a = 0.35f;
        sr.color = c;
    }

    void OnMouseExit()
    {
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
    }
}