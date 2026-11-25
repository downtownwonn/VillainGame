using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Keeps gameplay navigation buttons tied to the screen edges and positions
/// inventory contents relative to the shared world-space inventory artwork.
/// </summary>
public sealed class GameplayResponsiveLayout : MonoBehaviour
{
    private const string InventorySpriteName = "인벤토리 임임임시";
    private const float ArrowEdgeInset = 40f;
    private const float InventoryVerticalOffsetFromCenter = -0.4f;

    private static GameplayResponsiveLayout instance;
    private int lastWidth;
    private int lastHeight;
    private float lastCanvasScale = -1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (instance != null)
        {
            instance.ApplyToScene(SceneManager.GetActiveScene());
            return;
        }

        GameObject runner = new GameObject("GameplayResponsiveLayout");
        instance = runner.AddComponent<GameplayResponsiveLayout>();
        DontDestroyOnLoad(runner);
        instance.ApplyToScene(SceneManager.GetActiveScene());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyToScene(scene);
    }

    private void Update()
    {
        float canvasScale = GetCurrentInventoryCanvasScale();
        if (lastWidth == Screen.width && lastHeight == Screen.height &&
            Mathf.Approximately(lastCanvasScale, canvasScale))
            return;

        ApplyToScene(SceneManager.GetActiveScene());
    }

    private void ApplyToScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        Camera gameplayCamera = Camera.main;
        SpriteRenderer inventoryArtwork = FindInventoryArtwork(scene);
        RectTransform inventoryPanel = FindInventoryPanel(scene);

        if (gameplayCamera != null && inventoryArtwork != null && inventoryPanel != null)
            PositionInventoryPanel(inventoryArtwork, inventoryPanel, gameplayCamera);

        PositionNavigationArrows(scene);

        lastWidth = Screen.width;
        lastHeight = Screen.height;
        lastCanvasScale = GetCurrentInventoryCanvasScale();
    }

    private static SpriteRenderer FindInventoryArtwork(Scene scene)
    {
        SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>(true);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.gameObject.scene == scene && renderer.sprite != null &&
                renderer.sprite.name == InventorySpriteName)
                return renderer;
        }

        return null;
    }

    private static RectTransform FindInventoryPanel(Scene scene)
    {
        RectTransform[] rectTransforms = FindObjectsOfType<RectTransform>(true);
        foreach (RectTransform rectTransform in rectTransforms)
        {
            if (rectTransform.gameObject.scene == scene && rectTransform.name == "InventoryPanel")
                return rectTransform;
        }

        return null;
    }

    private static void PositionInventoryPanel(
        SpriteRenderer artwork,
        RectTransform panel,
        Camera gameplayCamera)
    {
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        RectTransform parentRect = panel.parent as RectTransform;
        if (canvas == null || parentRect == null)
            return;

        Bounds bounds = artwork.bounds;
        Vector3 screenCenter = gameplayCamera.WorldToScreenPoint(bounds.center);
        Vector3 screenTop = gameplayCamera.WorldToScreenPoint(
            new Vector3(bounds.center.x, bounds.max.y, bounds.center.z));
        Vector3 screenBottom = gameplayCamera.WorldToScreenPoint(
            new Vector3(bounds.center.x, bounds.min.y, bounds.center.z));

        if (screenCenter.z <= 0f || screenTop.z <= 0f || screenBottom.z <= 0f)
            return;

        float projectedHeight = Mathf.Abs(screenTop.y - screenBottom.y);
        Vector2 targetScreenPosition = new Vector2(
            screenCenter.x,
            screenCenter.y + projectedHeight * InventoryVerticalOffsetFromCenter);

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentRect, targetScreenPosition, uiCamera, out Vector3 targetWorldPosition))
            panel.position = targetWorldPosition;
    }

    private static void PositionNavigationArrows(Scene scene)
    {
        HashSet<Button> leftButtons = new HashSet<Button>();
        HashSet<Button> rightButtons = new HashSet<Button>();

        BackgroundChanger[] backgroundChangers = FindObjectsOfType<BackgroundChanger>(true);
        foreach (BackgroundChanger changer in backgroundChangers)
        {
            if (changer.gameObject.scene != scene)
                continue;

            if (changer.leftButton != null) leftButtons.Add(changer.leftButton);
            if (changer.rightButton != null) rightButtons.Add(changer.rightButton);
        }

        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.gameObject.scene != scene)
                continue;

            string objectName = button.gameObject.name.ToLowerInvariant();
            if (objectName.Contains("left arrow")) leftButtons.Add(button);
            if (objectName.Contains("right arrow")) rightButtons.Add(button);
        }

        foreach (Button button in leftButtons)
            AnchorArrow(button.transform as RectTransform, true);
        foreach (Button button in rightButtons)
            AnchorArrow(button.transform as RectTransform, false);
    }

    private static void AnchorArrow(RectTransform arrow, bool isLeft)
    {
        if (arrow == null || !(arrow.parent is RectTransform))
            return;

        Vector2 anchor = new Vector2(isLeft ? 0f : 1f, 0.5f);
        arrow.anchorMin = anchor;
        arrow.anchorMax = anchor;
        arrow.anchoredPosition = new Vector2(isLeft ? ArrowEdgeInset : -ArrowEdgeInset, 0f);
    }

    private static float GetCurrentInventoryCanvasScale()
    {
        RectTransform panel = FindInventoryPanel(SceneManager.GetActiveScene());
        Canvas canvas = panel != null ? panel.GetComponentInParent<Canvas>() : null;
        return canvas != null ? canvas.scaleFactor : -1f;
    }
}
