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

    // ���� ���õ� �������� ū �̹���
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

        // ū �̹��� �˾��� ó������ ����
        if (largeImagePopup != null)
        {
            largeImagePopup.SetActive(false);
        }

        // ó������ ������ ��ư�� ����
        if (magnifyButton != null)
        {
            magnifyButton.SetActive(false);
        }
    }

    // =========================================================
    // ���� �����ۿ� ���� Show �Լ�
    // �� �Լ��� ���� ����� �״�� ����
    // =========================================================
    public void Show(Sprite sprite, string name, string description)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ItemInfoPopup: ������ Sprite�� �����ϴ�.");
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
    // ��ȭ Ƽ��ó�� ū �̹����� �ʿ��� �����ۿ�
    // =========================================================
    public void ShowWithLargeImage(
        Sprite sprite,
        string name,
        string description,
        Sprite largeSprite)
    {
        // ���� �˾� ����� �״�� ���
        Show(sprite, name, description);

        // ū �̹��� ����
        currentLargeSprite = largeSprite;

        // ū �̹����� ���� ���� ������ ��ư ǥ��
        if (magnifyButton != null)
        {
            magnifyButton.SetActive(largeSprite != null);
        }
    }

    // =========================================================
    // ������ ��ư
    // =========================================================
    public void OpenLargeImage()
    {
        Debug.Log("������ ��ư Ŭ����!");

        if (currentLargeSprite == null)
        {
            Debug.LogWarning("ItemInfoPopup: Ȯ���� �̹����� �����ϴ�.");
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
            // ū �̹��� �˾� ���� �Ҹ�
            if (audioSource != null && largeOpenSound != null)
            {
                audioSource.PlayOneShot(largeOpenSound);
            }
        }
    }

    // =========================================================
    // ū �̹��� �˾� �ݱ�
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
    // ���� ���� �˾� �ݱ�
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

        // ū �̹��� �˾��� ���� �ݾ���
        if (largeImagePopup != null)
        {
            largeImagePopup.SetActive(false);
        }

        popup.SetActive(false);
        blocker.SetActive(false);

        // ���� �������� ���� �ʱ�ȭ
        currentLargeSprite = null;

        if (magnifyButton != null)
        {
            magnifyButton.SetActive(false);
        }
    }
}