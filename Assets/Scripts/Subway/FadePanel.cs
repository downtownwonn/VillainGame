using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SubwayFadeManager : MonoBehaviour
{
    public static SubwayFadeManager Instance;

    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        Instance = this;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            // 평소에는 클릭을 막지 않음
            fadeImage.raycastTarget = false;
        }
    }

    // =========================================================
    // 페이드 아웃
    // =========================================================

    public IEnumerator FadeOut(float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image가 연결되지 않았습니다.");
            yield break;
        }

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Clamp01(time / duration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // 페이드가 끝나도 클릭은 계속 통과
        fadeImage.raycastTarget = false;
    }

    // =========================================================
    // 페이드 인
    // =========================================================

    public IEnumerator FadeIn(float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image가 연결되지 않았습니다.");
            yield break;
        }

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            color.a = 1f - Mathf.Clamp01(time / duration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        // 다시 클릭 가능
        fadeImage.raycastTarget = false;
    }
}