using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>Applies the menu volume settings to scene audio sources as scenes load.</summary>
public sealed class GlobalSceneAudioVolume : MonoBehaviour
{
    private sealed class SourceState
    {
        public float baseVolume;
        public float appliedVolume;
        public bool isBgm;
    }

    private const string BgmKey = "GameBgmVolume";
    private const string SfxKey = "GameSfxVolume";
    private static GlobalSceneAudioVolume instance;
    private static float bgmVolume = 0.4f;
    private static float sfxVolume = 0.8f;
    private readonly Dictionary<AudioSource, SourceState> sources = new Dictionary<AudioSource, SourceState>();
    private readonly Dictionary<VideoPlayer, float[]> videoBaseVolumes = new Dictionary<VideoPlayer, float[]>();

    public static float BgmVolume => bgmVolume;
    public static float SfxVolume => sfxVolume;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        bgmVolume = PlayerPrefs.GetFloat(BgmKey, 0.4f);
        sfxVolume = PlayerPrefs.GetFloat(SfxKey, 0.8f);
        if (instance == null)
        {
            GameObject host = new GameObject("GlobalSceneAudioVolume");
            instance = host.AddComponent<GlobalSceneAudioVolume>();
            DontDestroyOnLoad(host);
        }
    }

    public static void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BgmKey, bgmVolume);
        if (instance != null) instance.RefreshSources();
    }

    public static void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxKey, sfxVolume);
        if (instance != null) instance.RefreshSources();
    }

    public static void Refresh()
    {
        if (instance != null) instance.RefreshSources();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshSources();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sources.Clear();
        videoBaseVolumes.Clear();
        RefreshSources();
    }

    private void Update()
    {
        RefreshSources();
    }

    private void RefreshSources()
    {
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include);
        HashSet<AudioSource> found = new HashSet<AudioSource>(allSources);
        foreach (AudioSource source in allSources)
        {
            if (source == null || IsMenuManagedSource(source)) continue;

            if (!sources.TryGetValue(source, out SourceState state))
            {
                state = new SourceState { baseVolume = source.volume, isBgm = IsBgmSource(source) };
                sources.Add(source, state);
            }
            float scale = state.isBgm ? EffectiveBgmVolume() : EffectiveSfxVolume();
            source.volume = state.baseVolume * scale;
            state.appliedVolume = source.volume;
        }

        List<AudioSource> removed = null;
        foreach (AudioSource tracked in sources.Keys)
        {
            if (tracked == null || !found.Contains(tracked))
            {
                if (removed == null) removed = new List<AudioSource>();
                removed.Add(tracked);
            }
        }
        if (removed != null)
        {
            foreach (AudioSource source in removed) sources.Remove(source);
        }

        VideoPlayer[] videos = FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include);
        HashSet<VideoPlayer> foundVideos = new HashSet<VideoPlayer>(videos);
        foreach (VideoPlayer video in videos)
        {
            if (video.audioOutputMode != VideoAudioOutputMode.Direct) continue;
            if (!videoBaseVolumes.TryGetValue(video, out float[] baseVolumes) || baseVolumes.Length != video.audioTrackCount)
            {
                baseVolumes = new float[video.audioTrackCount];
                for (ushort track = 0; track < video.audioTrackCount; track++)
                    baseVolumes[track] = video.GetDirectAudioVolume(track);
                videoBaseVolumes[video] = baseVolumes;
            }

            for (ushort track = 0; track < video.audioTrackCount; track++)
                video.SetDirectAudioVolume(track, baseVolumes[track] * EffectiveSfxVolume());
        }

        List<VideoPlayer> removedVideos = null;
        foreach (VideoPlayer video in videoBaseVolumes.Keys)
        {
            if (video == null || !foundVideos.Contains(video))
            {
                if (removedVideos == null) removedVideos = new List<VideoPlayer>();
                removedVideos.Add(video);
            }
        }
        if (removedVideos != null)
            foreach (VideoPlayer video in removedVideos) videoBaseVolumes.Remove(video);
    }

    private static bool IsMenuManagedSource(AudioSource source)
    {
        return source.GetComponent<MenuAudioManager>() != null;
    }

    private static float EffectiveBgmVolume()
    {
        return MenuAudioManager.IsBgmMuted() ? 0f : bgmVolume;
    }

    private static float EffectiveSfxVolume()
    {
        return MenuAudioManager.IsButtonMuted() ? 0f : sfxVolume;
    }

    private static bool IsBgmSource(AudioSource source)
    {
        string sceneName = source.gameObject.scene.name;
        if (IsStoryOrClear(sceneName)) return false;

        string objectName = source.gameObject.name;
        string clipName = source.clip != null ? source.clip.name : string.Empty;
        return objectName.IndexOf("bgm", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               objectName.IndexOf("music", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               clipName.IndexOf("bgm", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               clipName.IndexOf("music", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsStoryOrClear(string sceneName)
    {
        return sceneName.IndexOf("Story", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               sceneName.IndexOf("Clear", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
