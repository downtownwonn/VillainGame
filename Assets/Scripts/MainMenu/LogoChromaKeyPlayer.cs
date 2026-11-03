using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Plays the menu logo video into a UI RawImage and removes its green background.
/// The RawImage RectTransform controls the logo size and position in the Inspector.
/// </summary>
[DisallowMultipleComponent]
public sealed class LogoChromaKeyPlayer : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RawImage logoImage;

    [Header("Green Screen")]
    [SerializeField] private Material chromaKeyMaterial;
    [SerializeField] private Color keyColor = new Color(0f, 1f, 0f, 1f);
    [SerializeField, Range(0.01f, 0.6f)] private float keyThreshold = 0.22f;
    [SerializeField, Range(0.01f, 0.4f)] private float edgeSoftness = 0.1f;
    [SerializeField, Range(0.01f, 0.5f)] private float greenDominanceStart = 0.05f;
    [SerializeField, Range(0.02f, 0.8f)] private float greenDominanceEnd = 0.28f;
    [SerializeField, Range(0f, 1f)] private float spillReduction = 1f;

    private Material runtimeMaterial;

    private void Awake()
    {
        if (logoImage == null || videoPlayer == null || videoClip == null || renderTexture == null || chromaKeyMaterial == null)
        {
            Debug.LogError("LogoChromaKeyPlayer is missing a video, render texture, UI image, or chroma key material reference.", this);
            enabled = false;
            return;
        }

        logoImage.texture = renderTexture;
        logoImage.raycastTarget = false;

        runtimeMaterial = new Material(chromaKeyMaterial);
        ApplyKeySettings(runtimeMaterial);
        logoImage.material = runtimeMaterial;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        videoPlayer.Play();
    }

    private void OnValidate()
    {
        if (runtimeMaterial != null)
        {
            ApplyKeySettings(runtimeMaterial);
        }
    }

    private void ApplyKeySettings(Material material)
    {
        material.SetColor("_KeyColor", keyColor);
        material.SetFloat("_Threshold", keyThreshold);
        material.SetFloat("_Softness", edgeSoftness);
        material.SetFloat("_GreenDominanceStart", greenDominanceStart);
        material.SetFloat("_GreenDominanceEnd", greenDominanceEnd);
        material.SetFloat("_SpillReduction", spillReduction);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}
