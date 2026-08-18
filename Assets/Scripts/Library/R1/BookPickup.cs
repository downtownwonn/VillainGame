using UnityEngine;
using UnityEngine.UI;

public class BookPickup : MonoBehaviour
{
    public Sprite bookIcon;
    private SpriteRenderer sr;

    [Header("팝업 이미지 교체용")]
    public Image laptopPopupImage;      // 인스펙터에서 노트북 팝업의 Image 컴포넌트 연결
    public Sprite noSpeakerVersion;     // 스피커 없는 버전 이미지 연결

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(bookIcon);
        if (added)
        {
            gameObject.SetActive(false);

            // 팝업 배경 이미지를 스피커 없는 버전으로 교체
            if (laptopPopupImage != null && noSpeakerVersion != null)
            {
                laptopPopupImage.sprite = noSpeakerVersion;
            }
        }
    }

    void OnMouseEnter()
    {
        Color c = sr.color;
        c.a = 0.35f; // 마우스 올리면 살짝 보이게 (원하는 만큼 조정 가능)
        sr.color = c;
    }

    void OnMouseExit()
    {
        Color c = sr.color;
        c.a = 0f; // 마우스 떼면 다시 완전히 투명하게
        sr.color = c;
    }
}