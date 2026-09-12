using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInfoPopup : MonoBehaviour
{
    public static ItemInfoPopup Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject blocker;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [Header("Magnify")]
    [SerializeField] private GameObject magnifyButton;
    [SerializeField] private GameObject largeImagePopup;
    [SerializeField] private Image largeItemImage;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Large Image Popup Sound")]
    [SerializeField] private AudioClip largeOpenSound;
    [SerializeField] private AudioClip largeCloseSound;

    // 현재 선택된 아이템의 큰 이미지
    private Sprite currentLargeSprite;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        popup.SetActive(false);
        blocker.SetActive(false);

        // 큰 이미지 팝업도 처음에는 꺼둠
        if (largeImagePopup != null)
        {
            largeImagePopup.SetActive(false);
        }

        // 처음에는 돋보기 버튼도 꺼둠
        if (magnifyButton != null)
        {
            magnifyButton.SetActive(false);
        }
    }

    // =========================================================
    // 기존 아이템용 기존 Show 함수
    // 이 함수의 기존 기능은 그대로 유지
    // =========================================================
    public void Show(Sprite sprite, string name, string description)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ItemInfoPopup: 아이템 Sprite가 없습니다.");
            return;
        }

        itemImage.sprite = sprite;
        itemImage.preserveAspect = true;
        itemName.text = name;
        itemDescription.text = description;

        blocker.SetActive(true);
        popup.SetActive(true);

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

    // =========================================================
    // 영화 티켓처럼 큰 이미지가 필요한 아이템용
    // =========================================================
    public void ShowWithLargeImage(
        Sprite sprite,
        string name,
        string description,
        Sprite largeSprite)
    {
        // 기존 팝업 기능을 그대로 사용
        Show(sprite, name, description);

        // 큰 이미지 저장
        currentLargeSprite = largeSprite;

        // 큰 이미지가 있을 때만 돋보기 버튼 표시
        if (magnifyButton != null)
        {
            magnifyButton.SetActive(largeSprite != null);
        }
    }

    // =========================================================
    // 돋보기 버튼
    // =========================================================
    public void OpenLargeImage()
    {
        Debug.Log("돋보기 버튼 클릭됨!");

        if (currentLargeSprite == null)
        {
            Debug.LogWarning("ItemInfoPopup: 확대할 이미지가 없습니다.");
            return;
        }

        if (largeItemImage != null)
        {
            largeItemImage.sprite = currentLargeSprite;
            largeItemImage.preserveAspect = true;
        }

        if (largeImagePopup != null)
        {
            largeImagePopup.SetActive(true);
            // 큰 이미지 팝업 열기 소리
            if (audioSource != null && largeOpenSound != null)
            {
                audioSource.PlayOneShot(largeOpenSound);
            }
        }
    }

    // =========================================================
    // 큰 이미지 팝업 닫기
    // =========================================================
    public void CloseLargeImage()
    {
        if (largeImagePopup != null)
        {
            if (audioSource != null && largeCloseSound != null)
            {
                audioSource.PlayOneShot(largeCloseSound);
            }

            largeImagePopup.SetActive(false);
        }
    }

    // =========================================================
    // 기존 설명 팝업 닫기
    // =========================================================
    public void Close()
    {
        if (!popup.activeSelf)
        {
            return;
        }

        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        // 큰 이미지 팝업도 같이 닫아줌
        if (largeImagePopup != null)
        {
            largeImagePopup.SetActive(false);
        }

        popup.SetActive(false);
        blocker.SetActive(false);

        // 다음 아이템을 위해 초기화
        currentLargeSprite = null;

        if (magnifyButton != null)
        {
            magnifyButton.SetActive(false);
        }
    }
}