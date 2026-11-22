using UnityEngine;

public class LegZoomLoopAudio : MonoBehaviour
{
    [Header("확대 패널 반복 효과음")]
    [SerializeField] private AudioClip footTappingClip;
    [SerializeField, Min(0.1f)] private float playbackSpeed = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.clip = footTappingClip;
        audioSource.pitch = playbackSpeed;
    }

    private void OnEnable()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (footTappingClip == null) return;

        audioSource.clip = footTappingClip;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.pitch = Mathf.Max(0.1f, playbackSpeed);
        audioSource.Play();
    }

    private void OnDisable()
    {
        StopLoop();
    }

    public void StopLoop()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.Stop();
    }
}
