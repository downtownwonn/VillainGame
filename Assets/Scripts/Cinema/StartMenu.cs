using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private string stageSelectSceneName = "StageSelect";
    [SerializeField] private TMP_Text statusText;

    [Header("Menu Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private TMP_FontAsset menuFont;
    [Header("Volume Icons")]
    [Tooltip("Optional sprite for the circular BGM and SFX volume slider knobs.")]
    [SerializeField] private Sprite volumeKnobSprite;

    private void Awake()
    {
        MenuAudioManager.Initialize(bgmSource, bgmClip, buttonClickClip);
        ApplyMenuTheme();
        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(MenuAudioManager.GetBgmVolume());
        }
    }

    private void ApplyMenuTheme()
    {
        MenuUITheme.LoadAudioControlIconSet();

        Transform uiRoot = bgmSlider != null ? bgmSlider.transform.parent : null;
        if (uiRoot == null)
        {
            return;
        }

        StyleMenuButton(uiRoot.Find("StartButton"));
        StyleMenuButton(uiRoot.Find("QuitButton"));

        TMP_Text bgmLabel = null;
        foreach (TMP_Text text in uiRoot.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.text == "BGM VOLUME")
            {
                bgmLabel = text;
                break;
            }
        }

        if (bgmLabel != null)
        {
            bgmLabel.gameObject.SetActive(false);
        }

        bgmSlider.gameObject.SetActive(false);
        MenuUITheme.CreateAudioMuteButton(uiRoot, true, new Vector2(806.16f, 465f), null, null);
        MenuUITheme.CreateAudioMuteButton(uiRoot, false, new Vector2(892.72f, 465f), null, null);
    }

    private void StyleMenuButton(Transform buttonTransform)
    {
        if (buttonTransform == null)
        {
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();
        MenuUITheme.StyleButton(button, menuFont);
        Image buttonGraphic = button != null ? button.targetGraphic as Image : null;
        if (buttonGraphic != null)
        {
            buttonGraphic.sprite = null;
            buttonGraphic.color = Color.clear;
        }

        MenuHoverScale hover = buttonTransform.GetComponent<MenuHoverScale>();
        if (hover == null) hover = buttonTransform.gameObject.AddComponent<MenuHoverScale>();
        hover.SetTarget(buttonTransform as RectTransform);
        TMP_Text label = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.gameObject.SetActive(true);
            label.fontSize = 42f;
            label.color = Color.black;
        }
    }

    public void StartGame()
    {
        MenuAudioManager.PlayButtonClick();
        if (Application.CanStreamedLevelBeLoaded(stageSelectSceneName))
        {
            SceneTransitionManager.BeginTransition(stageSelectSceneName);
            return;
        }

        if (statusText != null)
        {
            statusText.text = $"{stageSelectSceneName} 씬은 아직 준비되지 않았습니다.";
        }

        Debug.Log($"'{stageSelectSceneName}' 씬을 Build Profiles에 추가하면 START 버튼이 해당 씬으로 이동합니다.");
    }

    public void QuitGame()
    {
        MenuAudioManager.PlayButtonClick();
        StartCoroutine(QuitAfterClick());
    }

    private IEnumerator QuitAfterClick()
    {
        yield return new WaitForSecondsRealtime(0.12f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetBgmVolume(float volume)
    {
        MenuAudioManager.SetBgmVolume(volume);
    }
}
