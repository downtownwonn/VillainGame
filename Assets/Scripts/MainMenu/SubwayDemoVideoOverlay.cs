using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>Plays a full-screen story or clear video, then advances to the configured scene.</summary>
[DisallowMultipleComponent]
public sealed class SubwayDemoVideoOverlay : MonoBehaviour
{
    [SerializeField] private VideoClip demoClip;
    [SerializeField] private bool loopVideo;
    [SerializeField] private string nextSceneName;

    private RenderTexture renderTexture;
    private GameObject canvasObject;
    private VideoPlayer videoPlayer;

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

        videoPlayer.Prepare();
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
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneTransitionManager.BeginTransition(nextSceneName);
        }
        else
        {
            Debug.LogError($"Demo video finished, but scene '{nextSceneName}' is not in Build Profiles.", this);
        }
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
