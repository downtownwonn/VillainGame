using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KissHeartEffect : MonoBehaviour
{
    [Header("Heart")]
    [Tooltip("뿅뿅 튀어나올 하트 프리팹")]
    public GameObject heartPrefab;


    [Header("Spawn")]
    [Tooltip("하트가 처음 생성되는 위치")]
    public Transform spawnPoint;


    [Header("Motion")]
    [Tooltip("하트가 위로 올라가는 거리")]
    public float moveDistance = 1.0f;

    [Tooltip("하트 하나가 나타났다 사라지는 시간")]
    public float moveDuration = 1.0f;

    [Tooltip("좌우로 퍼지는 정도")]
    public float randomX = 0.3f;

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


    [Header("Spawn Timing")]
    [Tooltip("하트 생성 간격")]
    public float spawnInterval = 0.7f;


    // 현재 생성되어 살아있는 하트들
    private readonly List<GameObject> activeHearts =
        new List<GameObject>();


    // 현재 하트를 보여줄지 여부
    private bool heartVisible = false;


    // ==========================================
    // 시작
    // ==========================================

    private void Start()
    {
        Debug.Log(
            "하트 효과 시작! Time.timeScale = " +
            Time.timeScale
        );

        // 게임 시작 시 한 번만 시작한다.
        // 배경 이동과 관계없이 계속 실행된다.
        StartCoroutine(HeartLoop());
    }


    // ==========================================
    // 하트 생성 루프
    // ==========================================

    private IEnumerator HeartLoop()
    {
        while (true)
        {
            SpawnHeart();

            // Time.timeScale의 영향을 받지 않도록
            // Realtime 사용
            yield return new WaitForSecondsRealtime(
                spawnInterval
            );
        }
    }


    // ==========================================
    // 하트 생성
    // ==========================================

    private void SpawnHeart()
    {
        if (heartPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning(
                "KissHeartEffect: " +
                "Heart Prefab 또는 Spawn Point가 없습니다."
            );

            return;
        }


        GameObject heart = Instantiate(
            heartPrefab,
            spawnPoint.position,
            Quaternion.identity
        );


        activeHearts.Add(heart);


        // ======================================
        // 현재 화면에서 하트를 보여줘야 하는지 적용
        // ======================================

        SpriteRenderer spriteRenderer =
            heart.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = heartVisible;

            // 혹시 프리팹 자체의 Alpha가 이전 상태로
            // 저장되어 있어도 새 하트는 정상적으로 시작
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }


        // 랜덤 회전
        float randomRotation =
            Random.Range(
                -randomAngle,
                randomAngle
            );

        heart.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                randomRotation
            );


        // 시작 크기
        heart.transform.localScale =
            Vector3.one * startScale;


        // 애니메이션 시작
        StartCoroutine(
            AnimateHeart(heart)
        );
    }


    // ==========================================
    // 하트 애니메이션
    // ==========================================

    private IEnumerator AnimateHeart(GameObject heart)
    {
        if (heart == null)
        {
            yield break;
        }


        Transform heartTransform =
            heart.transform;


        Vector3 startPosition =
            heartTransform.position;


        Vector3 targetPosition =
            startPosition +
            new Vector3(
                Random.Range(
                    -randomX,
                    randomX
                ),
                moveDistance,
                0f
            );


        // ======================================
        // POP
        // ======================================

        float timer = 0f;


        while (timer < popDuration)
        {
            if (heart == null)
            {
                yield break;
            }


            timer += Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    timer / popDuration
                );


            float scale =
                Mathf.Lerp(
                    startScale,
                    peakScale,
                    EaseOutBack(progress)
                );


            heartTransform.localScale =
                Vector3.one * scale;


            yield return null;
        }


        if (heart == null)
        {
            yield break;
        }


        heartTransform.localScale =
            Vector3.one * peakScale;


        // ======================================
        // MOVE + FADE
        // ======================================

        timer = 0f;


        float remainingDuration =
            moveDuration - popDuration;


        // 잘못된 값 방지
        remainingDuration =
            Mathf.Max(
                0.01f,
                remainingDuration
            );


        while (timer < remainingDuration)
        {
            if (heart == null)
            {
                yield break;
            }


            timer += Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    timer / remainingDuration
                );


            // 위치 이동
            heartTransform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    EaseOutCubic(progress)
                );


            // 크기 감소
            float scale =
                Mathf.Lerp(
                    peakScale,
                    endScale,
                    progress
                );


            heartTransform.localScale =
                Vector3.one * scale;


            // 페이드아웃
            SpriteRenderer spriteRenderer =
                heart.GetComponent<SpriteRenderer>();


            if (spriteRenderer != null)
            {
                Color color =
                    spriteRenderer.color;


                color.a =
                    Mathf.Lerp(
                        1f,
                        0f,
                        progress
                    );


                spriteRenderer.color =
                    color;
            }


            yield return null;
        }


        // ======================================
        // 삭제
        // ======================================

        activeHearts.Remove(heart);


        if (heart != null)
        {
            Destroy(heart);
        }
    }


    // ==========================================
    // 하트 표시 / 숨김
    // ==========================================

    public void SetHeartVisible(bool visible)
    {
        heartVisible = visible;


        foreach (GameObject heart in activeHearts)
        {
            if (heart == null)
            {
                continue;
            }


            SpriteRenderer spriteRenderer =
                heart.GetComponent<SpriteRenderer>();


            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }
        }
    }


    // ==========================================
    // Ease Out Back
    // ==========================================

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;


        return 1f +
               c3 * Mathf.Pow(t - 1f, 3f) +
               c1 * Mathf.Pow(t - 1f, 2f);
    }


    // ==========================================
    // Ease Out Cubic
    // ==========================================

    private float EaseOutCubic(float t)
    {
        return 1f -
               Mathf.Pow(
                   1f - t,
                   3f
               );
    }


    // ==========================================
    // 중요
    //
    // 이 오브젝트는 BackgroundChanger에 의해
    // SetActive(false) 되지 않아야 한다.
    //
    // 정상적인 상황에서는 이 함수가 실행되지 않는다.
    // ==========================================

    private void OnDisable()
    {
        Debug.Log(
            "HeartEffect DISABLED"
        );


        StopAllCoroutines();


        for (
            int i = activeHearts.Count - 1;
            i >= 0;
            i--
        )
        {
            if (activeHearts[i] != null)
            {
                Destroy(activeHearts[i]);
            }
        }


        activeHearts.Clear();
    }


    private void OnDestroy()
    {
        Debug.Log(
            "HeartEffect DESTROYED"
        );
    }
}