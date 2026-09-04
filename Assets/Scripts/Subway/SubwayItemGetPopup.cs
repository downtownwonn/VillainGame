using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubwayItemGetPopup : MonoBehaviour
{
    public static SubwayItemGetPopup Instance;

    public GameObject popupPanel;
    public Image getImage;
    public TMP_Text getText;

    private void Awake()
    {
        Instance = this;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void ShowGetPopup(string itemName, Sprite itemSprite)
    {
        if (popupPanel == null || getText == null)
            return;

        getText.text = itemName + " 획득!";

        if (getImage != null)
        {
            getImage.sprite = itemSprite;
        }

        popupPanel.SetActive(true);

        CancelInvoke(nameof(HideGetPopup));
        Invoke(nameof(HideGetPopup), 1.5f);
    }

    public void HideGetPopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}