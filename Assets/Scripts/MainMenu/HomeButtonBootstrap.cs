using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Adds the home button to gameplay scenes after their round intro finishes.</summary>
public static class HomeButtonBootstrap
{
    private const string StartSceneName = "StartS";
    private const string StageSelectSceneName = "StageSelect";
    private const string HomeCanvasName = "GlobalHomeButtonCanvas";
    private static CanvasGroup homeButtonCanvasGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsGameplayScene(scene.name))
        {
            GameObject existingCanvas = GameObject.Find(HomeCanvasName);
            if (existingCanvas != null)
                Object.Destroy(existingCanvas);
            homeButtonCanvasGroup = null;
            return;
        }

        if (GameObject.Find(HomeCanvasName) != null)
        {
            SetHomeButtonAlpha(1f);
        }
    }

    public static void ShowForGameplayScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!IsGameplayScene(scene.name))
            return;

        MenuAudioManager.Ensure(null, null);

        if (GameObject.Find(HomeCanvasName) != null)
        {
            SetHomeButtonAlpha(1f);
            return;
        }

        Sprite homeSprite = MenuUITheme.ConfiguredHomeSprite;
        if (homeSprite == null)
            homeSprite = Resources.Load<Sprite>("HomeButton");
        if (homeSprite == null)
        {
            Debug.LogError("Home button sprite not found at Resources/HomeButton.");
            return;
        }

        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("HomeButtonEventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        GameObject canvasObject = new GameObject(HomeCanvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject buttonObject = new GameObject("HomeButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(canvasObject.transform, false);
        homeButtonCanvasGroup = buttonObject.AddComponent<CanvasGroup>();

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-32f, -28f);
        rect.sizeDelta = new Vector2(70.56f, 70.56f);

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = homeSprite;
        // The configured home sprite is full-color artwork, not a monochrome
        // glyph; the white-gaps shader would turn its opaque pixels black.
        image.material = null;
        image.preserveAspect = true;
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => SceneManager.LoadScene(StartSceneName));

        // Keep the same two volume controls available during gameplay, just left of Home.
        Slider bgmControl = MenuUITheme.CreateVerticalAudioControl(
            canvas.transform, true, new Vector2(719.6f, 476.72f), new Vector2(719.6f, 361f), 127f,
            MenuAudioManager.GetBgmVolume(), MenuAudioManager.SetBgmVolume,
            whiteCinemaTheme: scene.name.StartsWith("Cinema"));
        Slider sfxControl = MenuUITheme.CreateVerticalAudioControl(
            canvas.transform, false, new Vector2(806.16f, 476.72f), new Vector2(806.16f, 361f), 127f,
            MenuAudioManager.GetButtonVolume(), MenuAudioManager.SetButtonVolume,
            whiteCinemaTheme: scene.name.StartsWith("Cinema"));
        AudioControlGroup controlGroup = canvasObject.AddComponent<AudioControlGroup>();
        controlGroup.Configure(canvas.transform, rect, bgmControl, sfxControl);
    }

    private static bool IsGameplayScene(string sceneName)
    {
        return sceneName != StartSceneName && sceneName != StageSelectSceneName &&
               (sceneName.Contains("Play") || sceneName == "Library R1" || sceneName == "Library R2");
    }

    public static void SetCinemaHomeButtonAlpha(float alpha)
    {
        if (!SceneManager.GetActiveScene().name.StartsWith("Cinema"))
            return;

        SetHomeButtonAlpha(alpha);
    }

    private static void SetHomeButtonAlpha(float alpha)
    {
        if (homeButtonCanvasGroup == null)
        {
            GameObject canvasObject = GameObject.Find(HomeCanvasName);
            Transform buttonTransform = canvasObject != null ? canvasObject.transform.Find("HomeButton") : null;
            if (buttonTransform != null)
            {
                homeButtonCanvasGroup = buttonTransform.GetComponent<CanvasGroup>();
                if (homeButtonCanvasGroup == null)
                    homeButtonCanvasGroup = buttonTransform.gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (homeButtonCanvasGroup != null)
            homeButtonCanvasGroup.alpha = Mathf.Clamp01(alpha);
    }

}


