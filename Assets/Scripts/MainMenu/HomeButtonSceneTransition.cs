using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fades gameplay out before returning to the start scene.</summary>
public sealed class HomeButtonSceneTransition : MonoBehaviour
{
    private const float FadeDuration = 2f;
    private Image fadeImage;
    private CanvasGroup[] controlGroups;
    private bool isTransitioning;

    public void Configure(Image overlay, CanvasGroup[] controls)
    {
        fadeImage = overlay;
        controlGroups = controls;
    }

    public void GoToStartScene(Button homeButton, string startSceneName)
    {
        if (isTransitioning || fadeImage == null)
            return;

        if (!SceneTransitionManager.PrepareScene(startSceneName))
            return;

        isTransitioning = true;
        if (homeButton != null)
            homeButton.interactable = false;

        StartCoroutine(FadeAndLoad(startSceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        Coroutine controlsFade = StartCoroutine(FadeControls(FadeDuration));

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        float elapsed = 0f;
        while (elapsed < FadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsed / FadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        if (controlsFade != null)
            yield return controlsFade;

        color.a = 1f;
        fadeImage.color = color;
        SceneTransitionManager.ActivatePreparedScene();
    }

    public IEnumerator FadeControls(float duration)
    {
        if (controlGroups == null || controlGroups.Length == 0)
            yield break;

        float[] startingAlphas = new float[controlGroups.Length];
        for (int i = 0; i < controlGroups.Length; i++)
            startingAlphas[i] = controlGroups[i] != null ? controlGroups[i].alpha : 0f;

        if (duration <= 0f)
        {
            SetControlsAlpha(0f, startingAlphas);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetControlsAlpha(Mathf.Clamp01(elapsed / duration), startingAlphas);
            yield return null;
        }

        SetControlsAlpha(1f, startingAlphas);
    }

    private void SetControlsAlpha(float fadeProgress, float[] startingAlphas)
    {
        for (int i = 0; i < controlGroups.Length; i++)
        {
            if (controlGroups[i] != null)
                controlGroups[i].alpha = Mathf.Lerp(startingAlphas[i], 0f, fadeProgress);
        }
    }
}
