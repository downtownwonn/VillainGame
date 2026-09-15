using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KissHeartEffect : MonoBehaviour
{
    [Header("Heart")]
    [Tooltip("뿅뿅 튀어나올 하트 PNG")]
    public Image heartPrefab;

    [Header("Spawn")]
    [Tooltip("하트가 생성될 위치")]
    public RectTransform spawnArea;

    [Tooltip("하트 생성 간격")]
    public float spawnInterval = 0.4f;

    [Header("Motion")]
    [Tooltip("하트가 위로 올라가는 거리")]
    public float moveDistance = 80f;

    [Tooltip("하트 하나가 나타났다 사라지는 시간")]
    public float moveDuration = 1f;

    [Tooltip("좌우로 퍼지는 정도")]
    public float randomX = 30f;

    [Tooltip("하트가 좌우로 기울어지는 최대 각도")]
    public float randomAngle = 15f;

    [Header("Scale")]
    [Tooltip("처음 등장했을 때 크기")]
    public float startScale = 0.2f;

    [Tooltip("뿅! 하고 커졌을 때 최대 크기")]
    public float peakScale = 1.15f;

    [Tooltip("사라질 때 크기")]
    public float endScale = 0.7f;

    [Header("Timing")]
    [Tooltip("뿅! 하고 커지는 데 걸리는 시간")]
    public float popDuration = 0.15f;

    private void Start()
    {
        StartCoroutine(HeartLoop());
    }

    private IEnumerator HeartLoop()
    {
        while (true)
        {
            SpawnHeart();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnHeart()
    {
        if (heartPrefab == null || spawnArea == null)
        {
            Debug.LogWarning("KissHeartEffect: Heart Prefab 또는 Spawn Area가 없습니다.");
            return;
        }

        Image heart = Instantiate(
            heartPrefab,
            spawnArea
        );

        RectTransform rect = heart.rectTransform;

        // =====================================
        // 시작 위치
        // =====================================

        rect.anchoredPosition = new Vector2(
            Random.Range(-randomX, randomX),
            0f
        );

        // =====================================
        // 시작 크기
        // =====================================

        rect.localScale =
            Vector3.one * startScale;

        // =====================================
        // 랜덤 회전
        // =====================================

        float randomRotation =
            Random.Range(-randomAngle, randomAngle);

        rect.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                randomRotation
            );

        // =====================================
        // 투명도 초기화
        // =====================================

        Color color = heart.color;
        color.a = 1f;
        heart.color = color;

        // =====================================
        // 애니메이션 시작
        // =====================================

        StartCoroutine(AnimateHeart(heart));
    }

    private IEnumerator AnimateHeart(Image heart)
    {
        RectTransform rect = heart.rectTransform;

        Vector2 startPosition =
            rect.anchoredPosition;

        float timer = 0f;

        // =====================================
        // ① 뿅! 하고 커지기
        // =====================================

        while (timer < popDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(timer / popDuration);

            // 부드럽게 커짐
            float scale =
                Mathf.Lerp(
                    startScale,
                    peakScale,
                    EaseOutBack(progress)
                );

            rect.localScale =
                Vector3.one * scale;

            yield return null;
        }

        // 최대 크기 확정
        rect.localScale =
            Vector3.one * peakScale;

        // =====================================
        // ② 위로 튀어오르면서 작아지고 사라짐
        // =====================================

        timer = 0f;

        float remainingDuration =
            moveDuration - popDuration;

        while (timer < remainingDuration)
        {
            timer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / remainingDuration
                );

            // -----------------------------
            // 위로 이동
            // -----------------------------

            float y =
                Mathf.Lerp(
                    0f,
                    moveDistance,
                    EaseOutCubic(progress)
                );

            // -----------------------------
            // 좌우 살짝 흔들림
            // -----------------------------

            float x =
                Mathf.Sin(
                    progress * Mathf.PI
                ) * Random.Range(-15f, 15f);

            rect.anchoredPosition =
                startPosition +
                new Vector2(x, y);

            // -----------------------------
            // 점점 작아짐
            // -----------------------------

            float scale =
                Mathf.Lerp(
                    peakScale,
                    endScale,
                    progress
                );

            rect.localScale =
                Vector3.one * scale;

            // -----------------------------
            // 점점 투명해짐
            // -----------------------------

            Color color = heart.color;

            color.a =
                Mathf.Lerp(
                    1f,
                    0f,
                    progress
                );

            heart.color = color;

            yield return null;
        }

        // =====================================
        // ③ 완전히 제거
        // =====================================

        Destroy(heart.gameObject);
    }

    // =====================================
    // Ease Out Back
    // 처음에 살짝 튀어나오는 느낌
    // =====================================

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f +
               c3 * Mathf.Pow(t - 1f, 3f) +
               c1 * Mathf.Pow(t - 1f, 2f);
    }

    // =====================================
    // Ease Out Cubic
    // 위로 갈수록 부드러워짐
    // =====================================

    private float EaseOutCubic(float t)
    {
        return 1f -
               Mathf.Pow(1f - t, 3f);
    }
}