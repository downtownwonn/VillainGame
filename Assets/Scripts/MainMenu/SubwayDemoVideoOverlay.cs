using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

/// <summary>Plays a full-screen story or clear video, then advances to the configured scene.</summary>
[DisallowMultipleComponent]
public sealed class SubwayDemoVideoOverlay : MonoBehaviour
{
    private const float SkipButtonShowDelay = 3f;
    private const float SkipButtonHideBeforeEnd = 1f;
    private const int SkipButtonSortingOrder = 50;

    [SerializeField] private VideoClip demoClip;
    [SerializeField] private bool loopVideo;
    [SerializeField] private string nextSceneName;

    private RenderTexture renderTexture;
    private GameObject canvasObject;
    private GameObject skipCanvasObject;
    private GameObject skipButtonObject;
    private VideoPlayer videoPlayer;
    private bool transitionRequested;

    private void Awake()
    {
        if (demoClip == null)
        {
            Debug.LogError("Subway demo video clip is not assigned.", this);
            enabled = false;
            return;
        }

        renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32)
        {
            name = "SubwayStoryDemoRuntimeTexture"
        };
        renderTexture.Create();

        // RenderTextures can contain undefined GPU memory until the video
        // decoder writes its first frame. Clear it so startup always shows black.
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = previousActive;

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = demoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        if (!loopVideo && !string.IsNullOrWhiteSpace(nextSceneName))
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        canvasObject = new GameObject("SubwayDemoVideoCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = -100;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject imageObject = new GameObject("DemoVideo", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(AspectRatioFitter));
        imageObject.transform.SetParent(canvasObject.transform, false);
        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = renderTexture;
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        AspectRatioFitter aspect = imageObject.GetComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        // Use the clip's actual dimensions. A fixed 16:9 ratio can leave bars
        // when an imported story clip uses a different aspect ratio.
        aspect.aspectRatio = GetClipAspectRatio(demoClip);

        CreateSkipButton();
        videoPlayer.Prepare();
    }

    private void CreateSkipButton()
    {
        Sprite skipSprite = Resources.Load<Sprite>("UI/SkipButton_GameStyle");
        if (skipSprite == null)
        {
            Debug.LogError("Skip button sprite was not found at Resources/UI/SkipButton_GameStyle.", this);
            return;
        }

        if (EventSystem.current == null)
        {
            GameObject eventSystemObject = new GameObject(
                "StoryVideoSkipEventSystem",
                typeof(EventSystem),
                typeof(InputSystemUIInputModule));
            eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        skipCanvasObject = new GameObject(
            "StoryVideoSkipCanvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas skipCanvas = skipCanvasObject.GetComponent<Canvas>();
        skipCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        skipCanvas.sortingOrder = SkipButtonSortingOrder;

        CanvasScaler scaler = skipCanvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject buttonObject = new GameObject(
            "SkipVideoButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(skipCanvasObject.transform, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.anchoredPosition = new Vector2(-32f, -28f);
        rect.sizeDelta = new Vector2(70.56f, 70.56f);

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = skipSprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(SkipVideo);

        skipButtonObject = buttonObject;
        skipButtonObject.SetActive(false);
    }

    private void Update()
    {
        if (skipButtonObject == null || videoPlayer == null || transitionRequested)
            return;

        double clipLength = demoClip.length;
        double playbackTime = videoPlayer.time;
        bool showSkipButton = videoPlayer.isPlaying &&
                              playbackTime >= SkipButtonShowDelay &&
                              clipLength - playbackTime > SkipButtonHideBeforeEnd;

        if (skipButtonObject.activeSelf != showSkipButton)
            skipButtonObject.SetActive(showSkipButton);
    }

    private static float GetClipAspectRatio(VideoClip clip)
    {
        if (clip != null && clip.height > 0)
            return (float)clip.width / clip.height;

        return 16f / 9f;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        ushort trackCount = source.audioTrackCount;
        source.controlledAudioTrackCount = trackCount;
        for (ushort track = 0; track < trackCount; track++)
        {
            source.EnableAudioTrack(track, true);
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
            source.SetTargetAudioSource(track, audioSource);
        }

        if (trackCount == 0)
            Debug.LogWarning($"Video '{demoClip.name}' has no imported audio track.", this);

        GlobalSceneAudioVolume.Refresh();
        source.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        TransitionToNextScene();
    }

    private void SkipVideo()
    {
        TransitionToNextScene();
    }

    private void TransitionToNextScene()
    {
        if (transitionRequested)
            return;

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError($"Demo video finished, but scene '{nextSceneName}' is not in Build Profiles.", this);
            return;
        }

        transitionRequested = true;
        if (skipButtonObject != null)
            skipButtonObject.SetActive(false);
        if (videoPlayer != null)
            videoPlayer.Pause();

        SceneTransitionManager.BeginTransition(nextSceneName);
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
