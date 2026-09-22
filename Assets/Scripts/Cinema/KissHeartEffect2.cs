using System.Collections;
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
    public float spawnInterval = 0.4f;

    private readonly System.Collections.Generic.List<GameObject> activeHearts =
    new System.Collections.Generic.List<GameObject>();

    private void Start()
    {
        Debug.Log("하트 효과 시작! Time.timeScale = " + Time.timeScale);

        StartCoroutine(HeartLoop());
    }


    private IEnumerator HeartLoop()
    {
        while (true)
        {
            Debug.Log("HeartLoop START");

            SpawnHeart();

            Debug.Log("WAIT START");

            yield return new WaitForSecondsRealtime(spawnInterval);

            Debug.Log("WAIT COMPLETE");
        }
    }

    private void SpawnHeart()
    {
        if (heartPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning(
                "KissHeartEffect: Heart Prefab 또는 Spawn Point가 없습니다."
            );

            return;
        }

        GameObject heart = Instantiate(
            heartPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
        activeHearts.Add(heart);

        Debug.Log(" 하트 생성됨!");

        float randomRotation =
            Random.Range(-randomAngle, randomAngle);

        heart.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                randomRotation
            );

        heart.transform.localScale =
            Vector3.one * startScale;

        StartCoroutine(AnimateHeart(heart));
    }

    private IEnumerator AnimateHeart(GameObject heart)
    {
        Transform heartTransform = heart.transform;

        Vector3 startPosition = heartTransform.position;

        Vector3 targetPosition =
            startPosition +
            new Vector3(
                Random.Range(-randomX, randomX),
                moveDistance,
                0f
            );

        float timer = 0f;

        Debug.Log("POP START");


        // Pop animation
        while (timer < popDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(timer / popDuration);

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

        heartTransform.localScale =
            Vector3.one * peakScale;

        Debug.Log("POP COMPLETE");


        // Move animation
        timer = 0f;

        float remainingDuration =
            moveDuration - popDuration;

        Debug.Log("MOVE START");


        while (timer < remainingDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / remainingDuration
                );


            heartTransform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    EaseOutCubic(progress)
                );


            float scale =
                Mathf.Lerp(
                    peakScale,
                    endScale,
                    progress
                );

            heartTransform.localScale =
                Vector3.one * scale;


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

                spriteRenderer.color = color;
            }

            yield return null;
        }


        Debug.Log("MOVE COMPLETE");

        activeHearts.Remove(heart);


        Destroy(heart);
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f +
               c3 * Mathf.Pow(t - 1f, 3f) +
               c1 * Mathf.Pow(t - 1f, 2f);
    }


    private float EaseOutCubic(float t)
    {
        return 1f -
               Mathf.Pow(1f - t, 3f);
    }

    private void OnDisable()
    {
        Debug.Log("HeartEffect DISABLED");

        StopAllCoroutines();

        for (int i = activeHearts.Count - 1; i >= 0; i--)
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
        Debug.Log("HeartEffect DESTROYED");
    }
}