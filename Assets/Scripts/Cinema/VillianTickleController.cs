using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VillainTickleController : MonoBehaviour
{
    [Header("Villain")]
    public GameObject villainObject;

    [Header("Tickle Settings")]
    public float tickleDuration = 1.5f;
    public float shakeAngle = 3f;
    public int shakeCount = 3;

    [Header("Feather Motion")]
    public float featherMoveSpeed = 12f;
    public float featherMoveAngle = 15f;

    [Header("Sounds")]
    public AudioSource tickleSound;
    public AudioSource walkingSound;

    [Header("Scene Fade")]
    public Image fadeImage;
    public float sceneFadeDuration = 1.5f;

    [Header("Next Scene")]
    public string nextSceneName = "CinemaR1Clear";

    private bool isTickling = false;

    private Quaternion originalRotation;

    public void StartTickle(GameObject draggedFeather)
    {
        if (isTickling)
            return;

        isTickling = true;

        if (villainObject == null)
            villainObject = gameObject;

        originalRotation = villainObject.transform.localRotation;

        StartCoroutine(TickleSequence(draggedFeather));
    }

    private IEnumerator TickleSequence(GameObject draggedFeather)
    {
        Debug.Log("① TickleSequence 시작");

        // =====================================
        // ① 간지럼 소리
        // =====================================

        if (tickleSound != null && tickleSound.enabled)
        {
            Debug.Log("② 간지럼 소리 재생");

            tickleSound.Play();
        }

        // =====================================
        // ② 깃털 + 빌런 간지럼
        // =====================================

        Transform feather = null;

        if (draggedFeather != null)
            feather = draggedFeather.transform;

        Vector3 featherStartPosition = Vector3.zero;

        if (feather != null)
            featherStartPosition = feather.position;

        float timer = 0f;

        Debug.Log("③ 간지럼 모션 시작");
        Quaternion featherStartRotation = Quaternion.identity;

        if (feather != null)
        {
            featherStartRotation = feather.localRotation;
        }

        bool fadeStarted = false;
        Coroutine screenFadeCoroutine = null;
        float fadeStartTime = Mathf.Max(0f, tickleDuration - sceneFadeDuration);

        while (timer < tickleDuration)
        {
            timer += Time.deltaTime;

            if (!fadeStarted && timer >= fadeStartTime)
            {
                fadeStarted = true;
                screenFadeCoroutine = StartCoroutine(FadeOutScreen(draggedFeather));
            }

            // =====================================
            // 깃털 회전
            // =====================================

            if (feather != null)
            {
                float featherAngle =
                    Mathf.Sin(timer * featherMoveSpeed)
                    * featherMoveAngle;

                feather.localRotation =
                    featherStartRotation *
                    Quaternion.Euler(0f, 0f, featherAngle);
            }

            // =====================================
            // 빌런 살짝 회전
            // =====================================


            yield return null;
        }

        Debug.Log("④ 간지럼 모션 끝");

        // 원래 회전으로 복구
        villainObject.transform.localRotation = originalRotation;

        // =====================================
        // ③ 깃털 인벤토리 복귀
        // =====================================

        if (draggedFeather != null)
        {
            DraggableItem item =
                draggedFeather.GetComponent<DraggableItem>();

            if (item != null)
            {
                Debug.Log("⑤ 깃털 인벤토리 복귀");

                item.ReturnToOriginalPosition();
            }
        }

        // =====================================
        // ④ 잠깐 대기
        // =====================================

        if (!fadeStarted)
        {
            screenFadeCoroutine = StartCoroutine(FadeOutScreen(draggedFeather));
        }

        if (screenFadeCoroutine != null)
            yield return screenFadeCoroutine;

        Debug.Log("⑦ 빌런 페이드아웃 완료");

        // =====================================
        // ⑥ 걷는 소리
        // =====================================

        Debug.Log("⑦ 빌런 페이드아웃 완료");

        // =====================================
        // ⑥ 걷는 소리
        // =====================================

        if (walkingSound != null &&
            walkingSound.enabled &&
            walkingSound.clip != null)
        {
            Debug.Log("⑧ 걷는 소리 재생");

            walkingSound.Play();

            Debug.Log(
                "⑨ 걷는 소리 길이: "
                + walkingSound.clip.length
                + "초"
            );

            yield return new WaitForSeconds(
                walkingSound.clip.length
            );
        }
        else
        {
            Debug.Log(" 걷는 소리가 연결되지 않았습니다.");
        }

        // =====================================
        // 빌런 비활성화
        // =====================================

        // =====================================
        // 화면 페이드아웃
        // =====================================

        Debug.Log("화면 페이드아웃 시작");



        Debug.Log("화면 페이드아웃 완료");

        // =====================================
        // 빌런 오브젝트 비활성화
        // =====================================

        Debug.Log("빌런 오브젝트 비활성화");

        villainObject.SetActive(false);

        // =====================================
        // 씬 전환
        // =====================================

        Debug.Log("CinemaR1Clear 씬으로 이동");

        SceneManager.LoadScene("CinemaR1Clear");
    }

    // =====================================
    // 빌런 페이드아웃
    // =====================================

    private IEnumerator FadeOutScreen(GameObject draggedFeather)
    {
        Graphic[] featherGraphics = draggedFeather != null
            ? draggedFeather.GetComponentsInChildren<Graphic>(true)
            : new Graphic[0];
        Color[] originalFeatherColors = new Color[featherGraphics.Length];

        for (int i = 0; i < featherGraphics.Length; i++)
        {
            if (featherGraphics[i] != null)
                originalFeatherColors[i] = featherGraphics[i].color;
        }

        if (fadeImage == null && featherGraphics.Length == 0)
            yield break;

        float duration = Mathf.Max(0f, sceneFadeDuration);
        float timer = 0f;

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = duration > 0f ? Mathf.Clamp01(timer / duration) : 1f;

            if (fadeImage != null)
            {
                Color fadeColor = fadeImage.color;
                fadeColor.a = alpha;
                fadeImage.color = fadeColor;
            }

            for (int i = 0; i < featherGraphics.Length; i++)
            {
                if (featherGraphics[i] == null)
                    continue;

                Color featherColor = originalFeatherColors[i];
                featherColor.a = Mathf.Lerp(originalFeatherColors[i].a, 0f, alpha);
                featherGraphics[i].color = featherColor;
            }

            yield return null;
        }

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 1f;
            fadeImage.color = fadeColor;
        }

        for (int i = 0; i < featherGraphics.Length; i++)
        {
            if (featherGraphics[i] == null)
                continue;

            Color featherColor = originalFeatherColors[i];
            featherColor.a = 0f;
            featherGraphics[i].color = featherColor;
        }
    }
}