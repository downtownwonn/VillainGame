using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetPopup : MonoBehaviour
{
    public static ItemGetPopup Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Image itemImage;

    [Header("Display")]
    [SerializeField] private float defaultDuration = 1.5f;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private float startScale = 0.8f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip getSound;

    private Coroutine popupCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        popup.SetActive(false);
        blocker.SetActive(false);
    }

    public void Show(Sprite sprite)
    {
        Show(sprite, defaultDuration);
    }

    public void Show(Sprite sprite, float duration)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ItemGetPopup: 아이템 Sprite가 설정되지 않았습니다.");
            return;
        }

        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        // 이번에 획득한 아이템 이미지로 변경
        itemImage.sprite = sprite;

        popupCoroutine = StartCoroutine(ShowPopup(duration));
    }

    private IEnumerator ShowPopup(float duration)
    {
        blocker.SetActive(true);
        popup.SetActive(true);

        popup.transform.localScale = Vector3.one * startScale;

        // 팝업이 나타날 때 효과음
        if (audioSource != null && getSound != null)
        {
            audioSource.PlayOneShot(getSound);
        }

        // 등장
        yield return StartCoroutine(
            ScalePopup(startScale, 1f, animationDuration)
        );

        // 유지
        yield return new WaitForSecondsRealtime(duration);

        // 퇴장
        yield return StartCoroutine(
            ScalePopup(1f, startScale, animationDuration)
        );

        popup.SetActive(false);
        blocker.SetActive(false);

        popupCoroutine = null;
    }

    private IEnumerator ScalePopup(float from, float to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(time / duration);
            progress = Mathf.SmoothStep(0f, 1f, progress);

            float scale = Mathf.Lerp(from, to, progress);

            popup.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        popup.transform.localScale = Vector3.one * to;
    }
}