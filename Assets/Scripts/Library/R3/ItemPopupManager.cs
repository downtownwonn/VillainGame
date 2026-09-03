using UnityEngine;
using UnityEngine.UI;

public class ItemPopupManager : MonoBehaviour
{
    public static ItemPopupManager Instance;

    public GameObject popupPanel;
    public Image popupImage;

    [Header("노트북 팝업일 때 스피커 존 활성화")]
    public Sprite laptopIconSprite;   // 노트북 인벤토리 아이콘 (MainItemPickup의 itemIcon과 같은 스프라이트)
    public GameObject speakerObject;  // 활성화할 speaker 오브젝트

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(Sprite sprite)
    {
        popupImage.sprite = sprite;
        popupPanel.SetActive(true);

        // 노트북 화면 팝업일 때만 스피커 존 클릭 가능하게
        if (laptopIconSprite != null && sprite == laptopIconSprite && speakerObject != null)
        {
            speakerObject.SetActive(true);
        }
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}