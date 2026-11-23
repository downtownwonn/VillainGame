using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Loads scenes in the background while a persistent cover hides activation.</summary>
public sealed class SceneTransitionManager : MonoBehaviour
{
    private const float RevealDuration = 0.22f;
    private static SceneTransitionManager instance;
    private static Sprite solidWhiteSprite;

    private Image coverImage;
    private AsyncOperation pendingLoad;
    private string pendingSceneName;
    private bool transitionStarted;
    private bool activationStarted;

    public static bool PrepareScene(string sceneName)
    {
        if (instance != null && instance.activationStarted)
            return false;

        if (instance != null && instance.pendingLoad != null && !instance.pendingLoad.isDone)
        {
            if (instance.pendingSceneName == sceneName)
                return true;

            Debug.LogWarning($"Scene transition to '{sceneName}' was ignored because '{instance.pendingSceneName}' is already loading.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is missing from Build Profiles.");
            return false;
        }

        SceneTransitionManager manager = CreateInstance();
        manager.pendingSceneName = sceneName;
        manager.coverImage.color = new Color(0f, 0f, 0f, 0f);
        manager.coverImage.raycastTarget = true;

        try
        {
            manager.pendingLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (manager.pendingLoad == null)
            {
                Debug.LogError($"Could not start loading scene '{sceneName}'.");
                Object.Destroy(manager.gameObject);
                return false;
            }

            manager.pendingLoad.allowSceneActivation = false;
            return true;
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            Object.Destroy(manager.gameObject);
            return false;
        }
    }

    public static void ActivatePreparedScene()
    {
        if (instance == null || instance.pendingLoad == null || instance.activationStarted)
            return;

        instance.activationStarted = true;
        instance.StartCoroutine(instance.ActivateAndReveal());
    }

    public static void BeginTransition(string sceneName, float fadeOutDuration = 0.28f)
    {
        if (!PrepareScene(sceneName))
            return;

        if (instance.transitionStarted || instance.activationStarted)
            return;

        instance.transitionStarted = true;
        instance.StartCoroutine(instance.FadeOutAndActivate(fadeOutDuration));
    }

    public static void BeginTransitionAfter(string sceneName, float delaySeconds)
    {
        if (string.IsNullOrEmpty(sceneName))
            return;

        SceneTransitionManager manager = CreateInstance();
        manager.coverImage.color = new Color(0f, 0f, 0f, 0f);
        manager.coverImage.raycastTarget = true;
        string sourceScenePath = SceneManager.GetActiveScene().path;
        manager.StartCoroutine(manager.BeginTransitionAfterRoutine(sceneName, delaySeconds, sourceScenePath));
    }

    private IEnumerator BeginTransitionAfterRoutine(string sceneName, float delaySeconds, string sourceScenePath)
    {
        if (delaySeconds > 0f)
            yield return new WaitForSecondsRealtime(delaySeconds);

        if (SceneManager.GetActiveScene().path != sourceScenePath)
        {
            coverImage.raycastTarget = false;
            yield break;
        }

        if (!PrepareScene(sceneName))
        {
            coverImage.raycastTarget = false;
            yield break;
        }

        // Keep input blocked through the delay and immediately into the fade.
        BeginTransition(sceneName);
    }

    private static SceneTransitionManager CreateInstance()
    {
        if (instance != null)
            return instance;

        GameObject host = new GameObject("SceneTransitionManager");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<SceneTransitionManager>();

        GameObject canvasObject = new GameObject("SceneTransitionCover", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(host.transform, false);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32767;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject imageObject = new GameObject("BlackCover", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(canvasObject.transform, false);
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        instance.coverImage = imageObject.GetComponent<Image>();
        if (solidWhiteSprite == null)
        {
            solidWhiteSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f));
        }
        instance.coverImage.sprite = solidWhiteSprite;
        instance.coverImage.type = Image.Type.Simple;
        instance.coverImage.color = new Color(0f, 0f, 0f, 0f);
        instance.coverImage.raycastTarget = false;
        return instance;
    }

    private IEnumerator FadeOutAndActivate(float duration)
    {
        coverImage.raycastTarget = true;
        float elapsed = 0f;
        Color color = coverImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            coverImage.color = color;
            yield return null;
        }

        color.a = 1f;
        coverImage.color = color;
        activationStarted = true;
        yield return ActivateAndReveal();
    }

    private IEnumerator ActivateAndReveal()
    {
        while (pendingLoad != null && pendingLoad.progress < 0.9f)
            yield return null;

        Color color = coverImage.color;
        color.a = 1f;
        coverImage.color = color;
        coverImage.raycastTarget = true;
        pendingLoad.allowSceneActivation = true;

        while (!pendingLoad.isDone)
            yield return null;

        yield return new WaitForEndOfFrame();

        float elapsed = 0f;
        while (elapsed < RevealDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / RevealDuration);
            coverImage.color = color;
            yield return null;
        }

        Object.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
