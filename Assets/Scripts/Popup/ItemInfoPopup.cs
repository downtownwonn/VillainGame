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

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

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
    }

    public void Show(Sprite sprite, string name, string description)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ItemInfoPopup: 아이템 Sprite가 없습니다.");
            return;
        }

        itemImage.sprite = sprite;
        itemName.text = name;
        itemDescription.text = description;

        blocker.SetActive(true);
        popup.SetActive(true);

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

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

        popup.SetActive(false);
        blocker.SetActive(false);
    }
}