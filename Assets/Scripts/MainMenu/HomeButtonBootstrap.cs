using System.Collections;
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
    private const string FadeCanvasName = "GlobalHomeSceneFadeCanvas";
    private const int ControlsSortingOrder = 10;
    private static Canvas controlsCanvas;
    private static CanvasGroup[] controlGroups;
    private static float[] modalRestoreAlphas;
    private static CanvasGroup homeButtonCanvasGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameplayPopupState.Clear();
        if (!IsGameplayScene(scene.name))
        {
            GameObject existingCanvas = GameObject.Find(HomeCanvasName);
            if (existingCanvas != null)
                Object.Destroy(existingCanvas);
            GameObject existingFadeCanvas = GameObject.Find(FadeCanvasName);
            if (existingFadeCanvas != null)
                Object.Destroy(existingFadeCanvas);
            homeButtonCanvasGroup = null;
            controlsCanvas = null;
            controlGroups = null;
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
        canvas.sortingOrder = ControlsSortingOrder;
        controlsCanvas = canvas;

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
        button.onClick.AddListener(() => MenuAudioManager.PlayControlClick(MenuUITheme.ConfiguredControlClickClip));

        // Keep the same two volume controls available during gameplay, just left of Home.
        Button bgmButton = MenuUITheme.CreateAudioMuteButton(
            canvas.transform, true, new Vector2(719.6f, 476.72f));
        Button sfxButton = MenuUITheme.CreateAudioMuteButton(
            canvas.transform, false, new Vector2(806.16f, 476.72f));

        // Keep the transition overlay above the gameplay controls. It stays
        // transparent and ignores clicks until the home button is pressed.
        GameObject fadeCanvasObject = new GameObject(FadeCanvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas fadeCanvas = fadeCanvasObject.GetComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 1000;
        CanvasScaler fadeScaler = fadeCanvasObject.GetComponent<CanvasScaler>();
        fadeScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        fadeScaler.referenceResolution = new Vector2(1920f, 1080f);
        fadeScaler.matchWidthOrHeight = 0.5f;
        GameObject fadeObject = new GameObject("HomeSceneFade", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fadeObject.transform.SetParent(fadeCanvasObject.transform, false);
        Image fadeImage = fadeObject.GetComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;
        RectTransform fadeRect = fadeObject.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = Vector2.zero;
        fadeRect.offsetMax = Vector2.zero;

        HomeButtonSceneTransition transition = canvasObject.AddComponent<HomeButtonSceneTransition>();
        transition.Configure(fadeImage, new[]
        {
            homeButtonCanvasGroup,
            bgmButton.GetComponent<CanvasGroup>(),
            sfxButton.GetComponent<CanvasGroup>()
        });
        controlGroups = new[] { homeButtonCanvasGroup, bgmButton.GetComponent<CanvasGroup>(), sfxButton.GetComponent<CanvasGroup>() };
        button.onClick.AddListener(() => transition.GoToStartScene(button, StartSceneName));
    }

    private static bool IsGameplayScene(string sceneName)
    {
        return sceneName != StartSceneName && sceneName != StageSelectSceneName &&
               (sceneName.Contains("Play") || sceneName == "Library R1" || sceneName == "Library R2" || sceneName == "Library R3");
    }

    public static void SetCinemaHomeButtonAlpha(float alpha)
    {
        if (!SceneManager.GetActiveScene().name.StartsWith("Cinema"))
            return;

        // While a modal is open, the popup controller applies one shared
        // opacity to all three controls. Do not dim Home a second time.
        if (GameplayPopupState.IsAnyOpen)
            return;

        SetHomeButtonAlpha(alpha);
    }

    public static IEnumerator FadeGameplayControls(float duration)
    {
        GameObject canvasObject = GameObject.Find(HomeCanvasName);
        HomeButtonSceneTransition transition = canvasObject != null
            ? canvasObject.GetComponent<HomeButtonSceneTransition>()
            : null;

        if (transition != null)
            yield return transition.FadeControls(duration);
    }

    public static void SetModalPopupOpen(Object owner, bool isOpen)
    {
        if (owner == null) return;
        GameplayPopupState.SetOpen(owner, isOpen);
    }

    public static void TrackModalPanel(GameObject panel)
    {
        if (panel != null && panel.GetComponent<GameplayModalPanel>() == null)
            panel.AddComponent<GameplayModalPanel>();
    }

    public static void RefreshPopupState()
    {
        bool modalOpen = GameplayPopupState.IsAnyOpen;
        if (controlsCanvas == null)
        {
            GameObject canvasObject = GameObject.Find(HomeCanvasName);
            if (canvasObject != null) controlsCanvas = canvasObject.GetComponent<Canvas>();
        }
        if (controlsCanvas != null)
            controlsCanvas.sortingOrder = modalOpen ? ControlsSortingOrder + 1 : ControlsSortingOrder;
        if (controlGroups == null || controlGroups.Length == 0)
        {
            GameObject canvasObject = GameObject.Find(HomeCanvasName);
            if (canvasObject != null)
            {
                Transform[] children = canvasObject.GetComponentsInChildren<Transform>(true);
                var groups = new System.Collections.Generic.List<CanvasGroup>();
                foreach (Transform child in children)
                    if (child.name == "HomeButton" || child.name == "MusicIcon" || child.name == "SoundIcon")
                    {
                        CanvasGroup group = child.GetComponent<CanvasGroup>();
                        if (group != null) groups.Add(group);
                    }
                controlGroups = groups.ToArray();
            }
        }
        if (controlGroups != null && modalOpen && modalRestoreAlphas == null)
        {
            modalRestoreAlphas = new float[controlGroups.Length];
            for (int i = 0; i < controlGroups.Length; i++)
                modalRestoreAlphas[i] = controlGroups[i] != null ? controlGroups[i].alpha : 1f;
        }
        if (controlGroups != null)
            foreach (CanvasGroup group in controlGroups)
                if (group != null)
                {
                    group.interactable = !modalOpen;
                    group.blocksRaycasts = !modalOpen;
                }
        if (controlGroups != null && !modalOpen && modalRestoreAlphas != null)
        {
            for (int i = 0; i < controlGroups.Length && i < modalRestoreAlphas.Length; i++)
                if (controlGroups[i] != null) controlGroups[i].alpha = modalRestoreAlphas[i];
            modalRestoreAlphas = null;
        }
        else if (controlGroups != null && modalOpen)
            foreach (CanvasGroup group in controlGroups)
                if (group != null) group.alpha = 0.5f;
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


