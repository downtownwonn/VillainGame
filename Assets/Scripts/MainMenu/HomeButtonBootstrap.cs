using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Adds a persistent home button to every scene except the two main menu screens.</summary>
public static class HomeButtonBootstrap
{
    private const string StartSceneName = "StartS";
    private const string StageSelectSceneName = "StageSelect";
    private const string HomeCanvasName = "GlobalHomeButtonCanvas";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == StartSceneName || scene.name == StageSelectSceneName)
            return;

        if (GameObject.Find(HomeCanvasName) != null)
            return;

        Sprite homeSprite = Resources.Load<Sprite>("HomeButton");
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

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-32f, -28f);
        rect.sizeDelta = new Vector2(67.2f, 67.2f);

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = scene.name.StartsWith("Cinema") ? CreateWhiteSprite(homeSprite) : homeSprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => SceneManager.LoadScene(StartSceneName));
    }

    private static Sprite CreateWhiteSprite(Sprite source)
    {
        Rect sourceRect = source.rect;
        int width = Mathf.RoundToInt(sourceRect.width);
        int height = Mathf.RoundToInt(sourceRect.height);
        Color[] pixels = source.texture.GetPixels(
            Mathf.RoundToInt(sourceRect.x), Mathf.RoundToInt(sourceRect.y), width, height);

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color(1f, 1f, 1f, pixels[i].a);

        Texture2D whiteTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        whiteTexture.SetPixels(pixels);
        whiteTexture.Apply();
        return Sprite.Create(whiteTexture, new Rect(0f, 0f, width, height),
            source.pivot / source.rect.size, source.pixelsPerUnit);
    }
}


