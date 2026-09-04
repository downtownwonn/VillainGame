using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubwayItemPopupManager : MonoBehaviour
{
    public static SubwayItemPopupManager Instance;

    [Header("팝업")]
    public GameObject popupPanel;
    public Image popupImage;
    public TMP_Text itemNameText;
    public TMP_Text descriptionText;

    private void Awake()
    {
        Instance = this;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void ShowPopup(Sprite sprite, ItemType itemType)
    {
        // 빈 슬롯이면 아무것도 하지 않음
        if (sprite == null)
            return;

        if (popupPanel == null || popupImage == null ||
            itemNameText == null || descriptionText == null)
        {
            Debug.LogWarning("SubwayItemPopupManager 연결이 제대로 되어 있지 않습니다.");
            return;
        }

        popupImage.sprite = sprite;

        switch (itemType)
        {
            case ItemType.HandleLeft:
                itemNameText.text = "지하철 손잡이";
                descriptionText.text = "손잡이에 웬 날이...?";
                break;

            case ItemType.HandleRight:
                itemNameText.text = "지하철 손잡이";
                descriptionText.text = "손잡이에 웬 날이...?";
                break;

            case ItemType.Scissors:
                itemNameText.text = "가위";
                descriptionText.text = "사용처를 알 것 같다";
                break;

            case ItemType.Sunglasses:
                itemNameText.text = "선글라스";
                descriptionText.text = "평범한 선글라스가 아니다";
                break;

            case ItemType.Sangmo:
                itemNameText.text = "상모";
                descriptionText.text = "원하는 사람이 있는 것 같다";
                break;

            case ItemType.Janggu:
                itemNameText.text = "장구";
                descriptionText.text = "풍물놀이에 사용되는 악기다";
                break;

            case ItemType.Bodyguards:
                itemNameText.text = "경호원 3인조";
                descriptionText.text = "누굴 경호하는 거지...?";
                break;

            default:
                itemNameText.text = "";
                descriptionText.text = "";
                break;
        }

        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}