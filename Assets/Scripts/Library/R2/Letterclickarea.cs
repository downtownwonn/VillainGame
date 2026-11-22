using UnityEngine;
using UnityEngine.UI;

public class LetterClickTrigger : MonoBehaviour
{
    [Header("편지 내용 패널")]
    public GameObject letterPanel;
    public SoundManager soundManager;

    [Header("팝업 배경")]
    [SerializeField, Range(0f, 1f)] private float backdropAlpha = 0.55f;

    private static GameObject sharedBackdrop;
    private static Sprite sharedWhiteSprite;
    private GameObject backdrop;
    private int originalPanelSiblingIndex;
    private bool panelWasOpen;

    private void Awake()
    {
        CreateBackdrop();
        SyncBackdropWithPanel();
    }

    private void LateUpdate()
    {
        SyncBackdropWithPanel();
    }

    void OnMouseDown()
    {
        if (letterPanel != null && !letterPanel.activeSelf)
        {
            if (soundManager != null) soundManager.PlayUIClick();
            HomeButtonBootstrap.TrackModalPanel(letterPanel);

            originalPanelSiblingIndex = letterPanel.transform.GetSiblingIndex();
            if (backdrop != null)
            {
                backdrop.SetActive(true);
                backdrop.transform.SetAsLastSibling();
            }
            letterPanel.transform.SetAsLastSibling();
            panelWasOpen = true;
            letterPanel.SetActive(true);
        }
    }

    private void CreateBackdrop()
    {
        if (letterPanel == null) return;

        if (sharedBackdrop != null)
        {
            backdrop = sharedBackdrop;
            return;
        }

        Canvas canvas = letterPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Transform canvasRoot = canvas.rootCanvas.transform;
        backdrop = new GameObject("Letter Popup Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = backdrop.GetComponent<RectTransform>();
        rect.SetParent(canvasRoot, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = backdrop.GetComponent<Image>();
        if (sharedWhiteSprite == null)
            sharedWhiteSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
        image.sprite = sharedWhiteSprite;
        image.color = new Color(0f, 0f, 0f, backdropAlpha);
        image.raycastTarget = true;
        backdrop.SetActive(false);
        sharedBackdrop = backdrop;
    }

    private void SyncBackdropWithPanel()
    {
        bool isOpen = letterPanel != null && letterPanel.activeInHierarchy;
        if (backdrop != null && backdrop.activeSelf != isOpen)
            backdrop.SetActive(isOpen);

        if (panelWasOpen && !isOpen && letterPanel != null)
        {
            letterPanel.transform.SetSiblingIndex(Mathf.Clamp(originalPanelSiblingIndex, 0, letterPanel.transform.parent.childCount - 1));
            panelWasOpen = false;
        }
    }

}
