using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Builds the temporary, replaceable stage selection UI.</summary>
[DisallowMultipleComponent]
public sealed class StageSelectScreen : MonoBehaviour
{
    [System.Serializable]
    private struct StageImageFraming
    {
        [Range(0f, 1f)] public float horizontalOffset;
        [Range(1f, 3f)] public float zoom;
    }

    [Header("Replaceable Stage Cards")]
    [SerializeField] private string[] stageLabels = { "Stage 1  영화관", "Stage 2  도서관", "Stage 3  지하철", "Stage 4  편의점" };
    [SerializeField] private string[] storySceneNames = { "CinemaR1Story", "LibraryR1Story", "SubwayR1Story", "StoreR1Story" };
    [SerializeField] private Sprite[] stageImages = new Sprite[4];
    [SerializeField] private Texture2D[] stageTitleTextures = new Texture2D[4];
    [Tooltip("One framing entry per stage in the same order as Stage Labels and Stage Images. Offset 0 = left, 0.5 = center, 1 = right.")]
    [SerializeField] private StageImageFraming[] stageImageFraming =
    {
        new StageImageFraming { horizontalOffset = 0.5f, zoom = 1f },
        new StageImageFraming { horizontalOffset = 0.5f, zoom = 1f },
        new StageImageFraming { horizontalOffset = 0.5f, zoom = 1f },
        new StageImageFraming { horizontalOffset = 0.5f, zoom = 1f }
    };
    [SerializeField] private TMP_FontAsset koreanFont;

    [Header("Fallback audio for opening this scene directly")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip stageHoverClip;
    [SerializeField] private AudioClip transitionWhooshClip;

    private readonly Color[] cardColors =
    {
        new Color(0.43f, 0.23f, 0.24f),
        new Color(0.22f, 0.39f, 0.28f),
        new Color(0.22f, 0.34f, 0.48f),
        new Color(0.54f, 0.38f, 0.20f)
    };

    private readonly Image[] generatedStageImages = new Image[4];
    private readonly Sprite[] generatedFullTitleSprites = new Sprite[4];
    private readonly RectTransform[] generatedStageCards = new RectTransform[4];
    private readonly float[] stageCardWidths = { 480f, 480f, 480f, 480f };
    private readonly float[] transitionStartCardWidths = new float[4];
    private RectTransform stageCanvasRect;
    private Image transitionFadeImage;
    private int hoveredStageIndex = -1;
    private int transitionStageIndex = -1;
    private float transitionProgress;
    private bool transitionInProgress;

    private void Awake()
    {
        MenuAudioManager.Ensure(bgmClip, buttonClickClip);
        BuildScreen();
    }

    private void BuildScreen()
    {
        if (FindFirstObjectByType<AudioListener>() == null)
        {
            new GameObject("MenuAudioListener", typeof(AudioListener));
        }

        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        GameObject canvasObject = new GameObject("StageSelectCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        stageCanvasRect = canvasObject.GetComponent<RectTransform>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image background = CreateImage("Background", canvas.transform, new Color(0.08f, 0.10f, 0.15f));
        Stretch(background.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        background.raycastTarget = false;

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            string label = i < stageLabels.Length ? stageLabels[i] : $"Stage {i + 1}";
            Sprite stageSprite = i < stageImages.Length ? stageImages[i] : null;
            Texture2D titleTexture = i < stageTitleTextures.Length ? stageTitleTextures[i] : null;
            Image image = CreateImage($"StageCard_{i + 1}", canvas.transform, cardColors[i]);
            RectTransform row = image.rectTransform;
            row.anchorMin = new Vector2(0.5f, 0f);
            row.anchorMax = new Vector2(0.5f, 1f);
            row.anchoredPosition = new Vector2((i + 0.5f) * 480f - 960f, 0f);
            row.sizeDelta = new Vector2(480f, 0f);
            generatedStageCards[i] = row;

            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => SelectStage(index));
            image.gameObject.AddComponent<RectMask2D>();

            EventTrigger hoverTrigger = image.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener(_ =>
            {
                if (hoveredStageIndex != index)
                {
                    hoveredStageIndex = index;
                    MenuAudioManager.PlayStageHover(stageHoverClip);
                }
            });
            hoverTrigger.triggers.Add(pointerEnter);
            EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener(_ =>
            {
                if (hoveredStageIndex == index)
                {
                    hoveredStageIndex = -1;
                }
            });
            hoverTrigger.triggers.Add(pointerExit);

            GameObject visualObject = new GameObject("StageCardVisual", typeof(RectTransform));
            visualObject.transform.SetParent(image.transform, false);
            RectTransform visualRect = visualObject.GetComponent<RectTransform>();
            Stretch(visualRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            if (stageSprite != null)
            {
                Image stageBackground = CreateImage("StageBackgroundImage", visualObject.transform, Color.white);
                stageBackground.sprite = stageSprite;
                stageBackground.raycastTarget = false;
                RectTransform pictureRect = stageBackground.rectTransform;
                pictureRect.anchorMin = pictureRect.anchorMax = new Vector2(0.5f, 0.5f);
                pictureRect.sizeDelta = Vector2.zero;
                generatedStageImages[i] = stageBackground;
            }

            float shadeAlpha = i == 0 ? 0.30f : 0.42f;
            Image shade = CreateImage("StageImageShade", visualObject.transform, new Color(0f, 0f, 0f, shadeAlpha));
            Stretch(shade.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            shade.raycastTarget = false;

            if (titleTexture != null)
            {
                Sprite fullTitleSprite = Sprite.Create(
                    titleTexture,
                    new Rect(0f, 0f, titleTexture.width, titleTexture.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
                fullTitleSprite.name = $"{titleTexture.name}_FullArtwork";
                generatedFullTitleSprites[i] = fullTitleSprite;

                Image titleImage = CreateImage("StageTitleImage", visualObject.transform, Color.white);
                titleImage.sprite = fullTitleSprite;
                titleImage.preserveAspect = true;
                titleImage.raycastTarget = false;
                RectTransform titleRect = titleImage.rectTransform;
                titleRect.anchorMin = titleRect.anchorMax = new Vector2(0.5f, 0.5f);
                titleRect.sizeDelta = new Vector2(330f, 240f);

                MenuHoverScale titleHover = image.gameObject.AddComponent<MenuHoverScale>();
                titleHover.SetTarget(titleRect);
                titleHover.SetHoverScale(1.2f);
            }
            else
            {
                CreateText(label.Replace("  ", "\n"), visualObject.transform, Vector2.zero, new Vector2(440f, 190f), 48, Color.white, FontStyles.Bold);
            }

        }

        // Navigation and volume remain small overlays over the full-height columns.
        Button backButton = CreateButton("BACK", canvas.transform, new Vector2(-790f, 480f), new Vector2(320f, 64f));
        backButton.gameObject.name = "BackButton";
        TMP_Text backLabel = backButton.GetComponentInChildren<TMP_Text>(true);
        if (backLabel != null)
        {
            backLabel.text = "< 돌아가기";
            backLabel.gameObject.name = "Text - 돌아가기";
            backLabel.fontSize = 42f;
            backLabel.enableWordWrapping = false;
            backLabel.overflowMode = TextOverflowModes.Overflow;
        }
        backButton.onClick.AddListener(BackToMainMenu);
        MenuUITheme.StyleButton(backButton, koreanFont);
        Image backButtonBackground = backButton.targetGraphic as Image;
        if (backButtonBackground != null)
        {
            backButtonBackground.sprite = null;
            backButtonBackground.color = Color.clear;
        }

        transitionFadeImage = CreateImage("StageTransitionFade", canvas.transform, new Color(0f, 0f, 0f, 0f));
        Stretch(transitionFadeImage.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        transitionFadeImage.raycastTarget = false;

    }

    private void LateUpdate()
    {
        UpdateStageCardWidths();

        for (int i = 0; i < generatedStageImages.Length; i++)
        {
            Image image = generatedStageImages[i];
            if (image == null || image.sprite == null || i >= stageImageFraming.Length)
            {
                continue;
            }

            ApplyImageFraming(image, image.sprite, stageImageFraming[i].horizontalOffset, stageImageFraming[i].zoom);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < generatedFullTitleSprites.Length; i++)
        {
            if (generatedFullTitleSprites[i] != null)
            {
                Destroy(generatedFullTitleSprites[i]);
            }
        }
    }

    private void UpdateStageCardWidths()
    {
        if (stageCanvasRect == null)
        {
            return;
        }

        float totalWidth = stageCanvasRect.rect.width;
        if (totalWidth <= 0f)
        {
            return;
        }

        float normalWidth = totalWidth / generatedStageCards.Length;
        float expandedWidth = normalWidth * 1.2f;
        float otherWidth = (totalWidth - expandedWidth) / (generatedStageCards.Length - 1);
        float interpolation = 1f - Mathf.Exp(-14f * Time.unscaledDeltaTime);

        float currentTotal = 0f;
        for (int i = 0; i < generatedStageCards.Length; i++)
        {
            if (generatedStageCards[i] == null)
            {
                continue;
            }

            if (transitionInProgress && transitionStageIndex >= 0)
            {
                float finalWidth = i == transitionStageIndex ? totalWidth : 0f;
                stageCardWidths[i] = Mathf.Lerp(transitionStartCardWidths[i], finalWidth, transitionProgress);
            }
            else
            {
                float targetWidth = i == hoveredStageIndex ? expandedWidth :
                    (hoveredStageIndex >= 0 ? otherWidth : normalWidth);
                stageCardWidths[i] = Mathf.Lerp(stageCardWidths[i], targetWidth, interpolation);
            }
            currentTotal += stageCardWidths[i];
        }

        if (currentTotal <= 0f)
        {
            return;
        }

        float widthCorrection = totalWidth / currentTotal;
        float leftEdge = -totalWidth * 0.5f;
        for (int i = 0; i < generatedStageCards.Length; i++)
        {
            RectTransform card = generatedStageCards[i];
            if (card == null)
            {
                continue;
            }

            float width = stageCardWidths[i] * widthCorrection;
            card.sizeDelta = new Vector2(width, 0f);
            card.anchoredPosition = new Vector2(leftEdge + width * 0.5f, 0f);
            leftEdge += width;
        }
    }

    private void ApplyImageFraming(Image image, Sprite sprite, float horizontalOffset, float zoom)
    {
        // Reapply in LateUpdate so Inspector tweaks and window resizing preview immediately in Play mode.
        RectTransform viewport = image.transform.parent.parent as RectTransform;
        if (viewport == null)
        {
            return;
        }

        float viewportWidth = viewport.rect.width;
        float viewportHeight = viewport.rect.height;
        if (viewportWidth <= 0f || viewportHeight <= 0f)
        {
            return;
        }

        float spriteWidth = sprite.rect.width;
        float spriteHeight = sprite.rect.height;
        float scale = Mathf.Max(viewportWidth / spriteWidth, viewportHeight / spriteHeight) * Mathf.Max(1f, zoom);
        float imageWidth = spriteWidth * scale;
        float imageHeight = spriteHeight * scale;

        RectTransform imageRect = image.rectTransform;
        imageRect.sizeDelta = new Vector2(imageWidth, imageHeight);
        imageRect.anchoredPosition = new Vector2((0.5f - Mathf.Clamp01(horizontalOffset)) * (imageWidth - viewportWidth), 0f);
    }

    private void SelectStage(int index)
    {
        if (transitionInProgress)
        {
            return;
        }

        MenuAudioManager.PlayButtonClick();
        if (index < 0 || index >= storySceneNames.Length || !Application.CanStreamedLevelBeLoaded(storySceneNames[index]))
        {
            Debug.LogError($"Stage {index + 1} Story Scene is missing from Build Profiles: " +
                           (index >= 0 && index < storySceneNames.Length ? storySceneNames[index] : "<unset>"));
            return;
        }

        transitionInProgress = true;
        transitionStageIndex = index;
        transitionProgress = 0f;
        for (int i = 0; i < generatedStageCards.Length; i++)
        {
            transitionStartCardWidths[i] = generatedStageCards[i] != null
                ? generatedStageCards[i].rect.width
                : 0f;
        }
        MenuAudioManager.PlayTransitionWhoosh(transitionWhooshClip);
        StartCoroutine(FadeToStoryScene(storySceneNames[index]));
    }

    private IEnumerator FadeToStoryScene(string sceneName)
    {
        transitionFadeImage.raycastTarget = true;
        const float transitionDuration = 4f;
        const float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            transitionProgress = Mathf.Clamp01(elapsed / transitionDuration);
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            transitionFadeImage.color = new Color(0f, 0f, 0f, alpha);
            MenuAudioManager.SetTransitionBgmFade(alpha);
            yield return null;
        }

        transitionProgress = 1f;
        transitionFadeImage.color = Color.black;
        MenuAudioManager.StopBgmForSceneTransition();
        SceneManager.LoadScene(sceneName);
    }

    private void BackToMainMenu()
    {
        MenuAudioManager.PlayButtonClick();
        if (Application.CanStreamedLevelBeLoaded("StartS"))
        {
            SceneManager.LoadScene("StartS");
        }
        else
        {
            Debug.LogError("MainMenu scene StartS is missing from Build Profiles.");
        }
    }

    private Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private TMP_Text CreateText(string value, Transform parent, Vector2 position, Vector2 size, float fontSize, Color color, FontStyles style)
    {
        GameObject go = new GameObject("Text - " + value, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TMP_Text text = go.GetComponent<TMP_Text>();
        text.text = value;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.raycastTarget = false;
        if (koreanFont != null)
        {
            text.font = koreanFont;
        }
        return text;
    }

    private Button CreateButton(string label, Transform parent, Vector2 position, Vector2 size)
    {
        Image image = CreateImage(label + "Button", parent, new Color(0.18f, 0.22f, 0.30f));
        image.rectTransform.sizeDelta = size;
        image.rectTransform.anchoredPosition = position;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        CreateText(label, image.transform, Vector2.zero, size, 28, Color.white, FontStyles.Bold);
        MenuHoverScale hover = image.gameObject.AddComponent<MenuHoverScale>();
        hover.SetTarget(image.rectTransform);
        return button;
    }

    private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
