using UnityEngine;

/// <summary>Persists the menu music and button sound while menu scenes change.</summary>
[DisallowMultipleComponent]
public sealed class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager Instance { get; private set; }

    [Header("Audio Clips (assign BGM in the Inspector when ready)")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip sliderReleaseClip;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.4f;
    [SerializeField, Range(0f, 1f)] private float buttonVolume = 0.8f;
    [SerializeField] private bool bgmMuted;
    [SerializeField] private bool buttonMuted;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource stageHoverSource;
    private AudioSource transitionSource;
    private float bgmTransitionMultiplier = 1f;
    private float lastStageHoverTime = -10f;
    private const float StageHoverCooldown = 0.22f;
    private const float StageHoverVolumeScale = 0.7f;

    public static void Initialize(AudioSource existingBgmSource, AudioClip music, AudioClip click)
    {
        if (Instance != null)
        {
            // Once initialized, keep the persistent source. A freshly loaded menu scene
            // passes its own scene-bound AudioSource here; adopting it would make the
            // music source get destroyed when that scene unloads.
            Instance.ApplyConfiguration(null, music, click);
            return;
        }

        GameObject host = existingBgmSource != null ? existingBgmSource.gameObject : new GameObject("MenuAudioManager");
        MenuAudioManager manager = host.GetComponent<MenuAudioManager>();
        if (manager == null)
        {
            manager = host.AddComponent<MenuAudioManager>();
        }

        manager.ApplyConfiguration(existingBgmSource, music, click);
    }

    public static void Ensure(AudioClip music, AudioClip click)
    {
        if (Instance == null)
        {
            Initialize(null, music, click);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f;
        bgmSource.volume = bgmMuted ? 0f : bgmVolume;

        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;

        stageHoverSource = gameObject.AddComponent<AudioSource>();
        stageHoverSource.playOnAwake = false;
        stageHoverSource.loop = false;
        stageHoverSource.spatialBlend = 0f;

        transitionSource = gameObject.AddComponent<AudioSource>();
        transitionSource.playOnAwake = false;
        transitionSource.loop = false;
        transitionSource.spatialBlend = 0f;
        transitionSource.pitch = 0.8f;
        ApplyConfiguration(null, bgmClip, buttonClickClip);
    }

    private void ApplyConfiguration(AudioSource existingBgmSource, AudioClip music, AudioClip click)
    {
        if (music != null || bgmClip == null)
        {
            bgmClip = music;
        }
        if (click != null)
        {
            buttonClickClip = click;
        }

        if (existingBgmSource != null && bgmSource != existingBgmSource)
        {
            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
            bgmSource = existingBgmSource;
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f;
            bgmSource.volume = bgmMuted ? 0f : bgmVolume;
        }

        if (bgmSource == null)
        {
            bgmSource = GetComponent<AudioSource>();
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }
        }

        bool changedMusic = bgmSource.clip != bgmClip;
        if (changedMusic)
        {
            bgmSource.Stop();
            bgmSource.clip = bgmClip;
        }

        if (bgmClip != null)
        {
            if (changedMusic || !bgmSource.isPlaying)
            {
                bgmTransitionMultiplier = 1f;
                bgmSource.Play();
            }
        }
        else if (changedMusic)
        {
            bgmSource.Stop();
        }

        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            sfxSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;
        }
    }

    public static void PlayButtonClick()
    {
        if (Instance != null && Instance.sfxSource != null && Instance.buttonClickClip != null && !Instance.buttonMuted)
        {
            Instance.sfxSource.PlayOneShot(Instance.buttonClickClip, Instance.buttonVolume);
        }
    }

    public static void PlaySliderRelease()
    {
        if (Instance != null && Instance.sfxSource != null && Instance.sliderReleaseClip != null && !Instance.buttonMuted)
        {
            Instance.sfxSource.PlayOneShot(Instance.sliderReleaseClip, Instance.buttonVolume);
        }
    }

    public static void PlayStageHover(AudioClip clip)
    {
        if (Instance == null || Instance.stageHoverSource == null || clip == null || Instance.buttonMuted)
        {
            return;
        }

        if (Time.unscaledTime - Instance.lastStageHoverTime < StageHoverCooldown)
        {
            return;
        }

        Instance.lastStageHoverTime = Time.unscaledTime;
        Instance.stageHoverSource.Stop();
        Instance.stageHoverSource.PlayOneShot(clip, Instance.buttonVolume * StageHoverVolumeScale);
    }

    public static void PlayTransitionWhoosh(AudioClip clip)
    {
        if (Instance == null || Instance.transitionSource == null || clip == null || Instance.buttonMuted)
        {
            return;
        }

        Instance.transitionSource.Stop();
        Instance.transitionSource.PlayOneShot(clip, Instance.buttonVolume);
    }

    public static void SetBgmVolume(float volume)
    {
        if (Instance == null)
        {
            return;
        }

        Instance.bgmVolume = Mathf.Clamp01(volume);
        Instance.ApplyBgmVolume();
    }

    public static void SetTransitionBgmFade(float fadeProgress)
    {
        if (Instance == null)
        {
            return;
        }

        Instance.bgmTransitionMultiplier = 1f - Mathf.Clamp01(fadeProgress);
        Instance.ApplyBgmVolume();
    }

    public static void StopBgmForSceneTransition()
    {
        if (Instance == null || Instance.bgmSource == null)
        {
            return;
        }

        Instance.bgmSource.Stop();
        Instance.bgmTransitionMultiplier = 1f;
        Instance.ApplyBgmVolume();
    }

    public static float GetBgmVolume()
    {
        return Instance != null ? Instance.bgmVolume : 0.4f;
    }

    public static bool ToggleBgmMute()
    {
        if (Instance == null) return false;
        Instance.bgmMuted = !Instance.bgmMuted;
        Instance.ApplyBgmVolume();
        return Instance.bgmMuted;
    }

    public static bool IsBgmMuted()
    {
        return Instance != null && Instance.bgmMuted;
    }

    public static void SetButtonVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.buttonVolume = Mathf.Clamp01(volume);
        }
    }

    public static bool ToggleButtonMute()
    {
        if (Instance == null) return false;
        Instance.buttonMuted = !Instance.buttonMuted;
        return Instance.buttonMuted;
    }

    public static bool IsButtonMuted()
    {
        return Instance != null && Instance.buttonMuted;
    }

    public static float GetButtonVolume()
    {
        return Instance != null ? Instance.buttonVolume : 0.8f;
    }

    private void ApplyBgmVolume()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmMuted ? 0f : bgmVolume * bgmTransitionMultiplier;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
