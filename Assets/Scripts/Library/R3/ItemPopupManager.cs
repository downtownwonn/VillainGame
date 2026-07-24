using UnityEngine;
using UnityEngine.UI;

public class ItemPopupManager : MonoBehaviour
{
    public static ItemPopupManager Instance;

    public GameObject popupPanel;   // TicketPopup 오브젝트
    public Image popupImage;        // 그 안의 Image

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(Sprite sprite)
    {
        popupImage.sprite = sprite;
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
