using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private string stageSelectSceneName = "StageSelect";
    [SerializeField] private TMP_Text statusText;

    [Header("Menu Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private TMP_FontAsset menuFont;
    [Header("Volume Icons")]
    [Tooltip("Optional sprite for the circular BGM and SFX volume slider knobs.")]
    [SerializeField] private Sprite volumeKnobSprite;

    private void Awake()
    {
        MenuAudioManager.Initialize(bgmSource, bgmClip, buttonClickClip);
        ApplyMenuTheme();
        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(MenuAudioManager.GetBgmVolume());
        }
    }

    private void Start()
    {
        // Reapply after every scene component has completed Awake so the startup
        // order cannot leave the persistent BGM source idle.
        if (bgmClip != null)
        {
            MenuAudioManager.Initialize(null, bgmClip, buttonClickClip);
        }

        // Older slider settings could have saved zero without setting mute.
        if (!MenuAudioManager.IsBgmMuted() && MenuAudioManager.GetBgmVolume() <= 0.001f)
        {
            MenuAudioManager.SetBgmVolume(0.4f);
        }
    }

    private void ApplyMenuTheme()
    {
        MenuUITheme.LoadAudioControlIconSet();

        Transform uiRoot = bgmSlider != null ? bgmSlider.transform.parent : null;
        if (uiRoot == null)
        {
            return;
        }

        StyleMenuButton(uiRoot.Find("StartButton"));
        RectTransform startRect = uiRoot.Find("StartButton") as RectTransform;
        Transform quitTransform = uiRoot.Find("QuitButton");
        StyleMenuButton(quitTransform);
        if (quitTransform != null && startRect != null)
        {
            RectTransform quitRect = quitTransform as RectTransform;
            startRect.anchoredPosition += Vector2.up * 30f;
            const float buttonSpacing = 85f;
            Vector2 creditsPosition = startRect.anchoredPosition + Vector2.down * buttonSpacing;
            quitRect.anchoredPosition = startRect.anchoredPosition + Vector2.down * (buttonSpacing * 2f);
            CreateCreditsButton(uiRoot, creditsPosition, quitRect.sizeDelta);
        }

        TMP_Text bgmLabel = null;
        foreach (TMP_Text text in uiRoot.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.text == "BGM VOLUME")
            {
                bgmLabel = text;
                break;
            }
        }

        if (bgmLabel != null)
        {
            bgmLabel.gameObject.SetActive(false);
        }

        bgmSlider.gameObject.SetActive(false);
        MenuUITheme.CreateAudioMuteButton(uiRoot, true, new Vector2(806.16f, 465f), null, null);
        MenuUITheme.CreateAudioMuteButton(uiRoot, false, new Vector2(892.72f, 465f), null, null);
    }

    private void CreateCreditsButton(Transform uiRoot, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject("CreditsButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.SetParent(uiRoot, false);
        buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;
        Image image = buttonObject.GetComponent<Image>();
        image.color = Color.clear;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(buttonRect, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
        TMP_Text label = labelObject.GetComponent<TMP_Text>();
        label.text = "CREDITS";
        label.font = menuFont;
        label.fontSize = 39f;
        label.color = Color.black;
        label.alignment = TextAlignmentOptions.Center;
        label.fontStyle = FontStyles.Bold;
        label.raycastTarget = false;
        MenuHoverScale hover = buttonObject.AddComponent<MenuHoverScale>();
        hover.SetTarget(buttonRect);
        GameObject overlay = CreateCreditsOverlay(uiRoot);
        button.onClick.AddListener(() =>
        {
            MenuAudioManager.PlayButtonClick();
            overlay.transform.SetAsLastSibling();
            overlay.SetActive(true);
        });
    }

    private GameObject CreateCreditsOverlay(Transform uiRoot)
    {
        GameObject overlay = new GameObject("CreditsOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.SetParent(uiRoot, false);
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = overlayRect.offsetMax = Vector2.zero;
        Image backdrop = overlay.GetComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.9f);
        backdrop.raycastTarget = true;

        TMP_Text title = CreateCreditsText(overlayRect, "한 방의 레시피\n참지마요  ·  CREDITS", new Vector2(0f, 320f), new Vector2(1500f, 130f), 38f, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        CreateCreditsScroll(overlayRect);
        CreateCreditsCloseButton(overlayRect);
        overlay.SetActive(false);
        return overlay;
    }

    private void CreateCreditsScroll(RectTransform parent)
    {
        GameObject viewportObject = new GameObject("CreditsViewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
        RectTransform viewport = viewportObject.GetComponent<RectTransform>();
        viewport.SetParent(parent, false);
        viewport.anchorMin = viewport.anchorMax = new Vector2(0.5f, 0.5f);
        viewport.anchoredPosition = new Vector2(0f, -140f);
        viewport.sizeDelta = new Vector2(1520f, 740f);
        Image viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;

        GameObject contentObject = new GameObject("CreditsContent", typeof(RectTransform));
        RectTransform content = contentObject.GetComponent<RectTransform>();
        content.SetParent(viewport, false);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 1000f);

        const string credits =
            "<b>Production Team</b>\n" +
            "Shin Hye-won — Stage 1, Team Leader; Sogang University, Art & Technology\n" +
            "Kang Seo-hyun — Stage 2; Sogang University, Art & Technology\n" +
            "Yang Seung-ha — Stage 3; Sogang University, Art & Technology\n" +
            "Kim Gyu-hwa — Stage 4; Sogang University, Art & Technology\n\n" +
            "<b>Production & Resource Credits</b>\n" +
            "Character and background artwork: Created using generative AI based on artwork drawn by our team.\n" +
            "Item images and game logo video: Created using generative AI.\n" +
            "Home button artwork: Generated with AI.\n" +
            "Volume icon: Sound icons by pojok d from Flaticon.\n" +
            "BGM icon: Music player icons by SumberRejeki from Flaticon.\n" +
            "Start screen BGM, ‘Comedy Funny’: leberch, Pixabay.\n" +
            "Gameplay BGM: Pixabay Content License.\n" +
            "Library R3 BGM, ‘Funk & Breakbeat’: AlexGuz, Pixabay.\n" +
            "Sound effects: Pixabay Content License.";
        TMP_Text body = CreateCreditsText(content, credits, Vector2.zero, new Vector2(1450f, 1000f), 22f, TextAlignmentOptions.TopLeft);
        body.margin = new Vector4(24f, 16f, 24f, 24f);
        body.lineSpacing = 8f;
        body.ForceMeshUpdate();
        RectTransform bodyRect = body.rectTransform;
        bodyRect.anchorMin = new Vector2(0f, 1f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.pivot = new Vector2(0.5f, 1f);
        bodyRect.anchoredPosition = new Vector2(0f, -20f);
        bodyRect.sizeDelta = new Vector2(0f, body.preferredHeight + 40f);
        content.sizeDelta = new Vector2(0f, Mathf.Max(740f, body.preferredHeight + 60f));
        ScrollRect scroll = viewportObject.GetComponent<ScrollRect>();
        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 35f;
    }

    private TMP_Text CreateCreditsText(RectTransform parent, string value, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject("CreditsText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.font = menuFont;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        return text;
    }

    private void CreateCreditsCloseButton(RectTransform parent)
    {
        GameObject closeObject = new GameObject("CreditsCloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        RectTransform rect = closeObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-55f, -45f);
        rect.sizeDelta = new Vector2(180f, 76f);
        Image image = closeObject.GetComponent<Image>();
        image.color = Color.clear;
        Button button = closeObject.GetComponent<Button>();
        button.targetGraphic = image;
        TMP_Text label = CreateCreditsText(rect, "CLOSE", Vector2.zero, rect.sizeDelta, 26f, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;
        button.onClick.AddListener(() =>
        {
            MenuAudioManager.PlayButtonClick();
            parent.gameObject.SetActive(false);
        });
    }

    private void StyleMenuButton(Transform buttonTransform)
    {
        if (buttonTransform == null)
        {
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();
        MenuUITheme.StyleButton(button, menuFont);
        Image buttonGraphic = button != null ? button.targetGraphic as Image : null;
        if (buttonGraphic != null)
        {
            buttonGraphic.sprite = null;
            buttonGraphic.color = Color.clear;
        }

        MenuHoverScale hover = buttonTransform.GetComponent<MenuHoverScale>();
        if (hover == null) hover = buttonTransform.gameObject.AddComponent<MenuHoverScale>();
        hover.SetTarget(buttonTransform as RectTransform);
        TMP_Text label = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.gameObject.SetActive(true);
            label.fontSize = buttonTransform.name == "StartButton" || buttonTransform.name == "QuitButton" ? 41f : label.fontSize;
            label.color = Color.black;
            if (buttonTransform.name == "QuitButton")
            {
                label.text = "나가기";
            }
            else if (buttonTransform.name == "StartButton")
            {
                label.text = "시작하기";
            }
        }
    }

    public void StartGame()
    {
        MenuAudioManager.PlayButtonClick();
        if (Application.CanStreamedLevelBeLoaded(stageSelectSceneName))
        {
            SceneTransitionManager.BeginTransition(stageSelectSceneName);
            return;
        }

        if (statusText != null)
        {
            statusText.text = $"{stageSelectSceneName} 씬은 아직 준비되지 않았습니다.";
        }

        Debug.Log($"'{stageSelectSceneName}' 씬을 Build Profiles에 추가하면 START 버튼이 해당 씬으로 이동합니다.");
    }

    public void QuitGame()
    {
        MenuAudioManager.PlayButtonClick();
        StartCoroutine(QuitAfterClick());
    }

    private IEnumerator QuitAfterClick()
    {
        yield return new WaitForSecondsRealtime(0.12f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetBgmVolume(float volume)
    {
        MenuAudioManager.SetBgmVolume(volume);
    }
}
