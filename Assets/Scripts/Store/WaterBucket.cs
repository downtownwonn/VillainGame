using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterBucket : MonoBehaviour
{
    [Header("Bini 설정")]
    [SerializeField] private GameObject hatObject;
    [Tooltip("비니 씻길 때 크기 비율")]
    public float biniScale = 0.5f;
    [Tooltip("비니 Y축 시작 위치 오프셋 (월드 단위)")]
    public float biniYOffset = 0.5f;
    [Tooltip("비니가 아래에서 위로 떠오르는 거리")]
    public float biniRiseHeight = 1.0f;
    [Tooltip("비니 기본 회전 각도 (Z축)")]
    public float biniRotationZ = 0f;

    [Header("Bini 1차 세척 타이밍")]
    public float bini1EnterDuration = 0.5f;
    public float bini1WaitDuration = 0.2f;
    public float bini1ExitDuration = 0.5f;

    [Header("Bini 2차 세척 타이밍")]
    public float bini2EnterDuration = 0.5f;
    public float bini2WaitDuration = 0.2f;
    public float bini2ExitDuration = 0.5f;

    [Header("Bini 3차 세척 타이밍 (깊이 포함)")]
    public float bini3EnterDuration = 0.5f;
    public float bini3WaitDuration = 1.0f;
    [Tooltip("세 번째로 들어갈 때의 깊이 (더 깊게 보내려면 값을 크게 설정)")]
    public float bini3SinkDepth = 1.5f; 

    [Header("Bini 공통 타이밍 설정")]
    public float bini1_2SinkDepth = 0.5f;
    public float biniRiseDuration = 0.8f;

    [Header("MopHandle 설정")]
    [SerializeField] private GameObject blackMopHandleObject;
    [Tooltip("자루 씻길 때 크기 비율")]
    public float mophandleScale = 0.8f; 
    [Tooltip("자루 Y축 시작 위치 오프셋 (월드 단위)")]
    public float mophandleYOffset = 0.5f;
    [Tooltip("자루가 아래에서 위로 떠오르는 거리")]
    public float mophandleRiseHeight = 1.0f;
    [Tooltip("자루 씻길 때 처음부터 적용될 Z축 회전 각도 (기본 90도)")]
    public float mophandleRotationZ = 90f;

    [Header("MopHandle 1차 세척 타이밍")]
    public float mophandle1EnterDuration = 0.5f;
    public float mophandle1WaitDuration = 0.2f;
    public float mophandle1ExitDuration = 0.5f;

    [Header("MopHandle 2차 세척 타이밍")]
    public float mophandle2EnterDuration = 0.5f;
    public float mophandle2WaitDuration = 0.2f;
    public float mophandle2ExitDuration = 0.5f;

    [Header("MopHandle 3차 세척 타이밍 (깊이 포함)")]
    public float mophandle3EnterDuration = 0.5f;
    public float mophandle3WaitDuration = 1.0f;
    [Tooltip("세 번째로 들어갈 때의 깊이 (더 깊게 보내려면 값을 크게 설정)")]
    public float mophandle3SinkDepth = 1.5f;

    [Header("MopHandle 공통 타이밍 설정")]
    public float mophandle1_2SinkDepth = 0.5f;
    public float mophandleRiseDuration = 0.8f;

    [Header("개별 사운드 설정 (단계별 입출수 효과음)")]
    public AudioClip wash1EnterSound;
    public AudioClip wash1ExitSound;
    public AudioClip wash2EnterSound;
    public AudioClip wash2ExitSound;
    public AudioClip wash3EnterSound;

    [Header("이펙트 설정")]
    [Tooltip("물 튀기는 효과 프리팹 (Particle System 등)")]
    public GameObject splashEffectPrefab;

    private bool isWashing = false;

    public void OnItemDropped(DraggableItemofStore draggableItem)
    {
        if (isWashing) return;

        if (draggableItem != null)
        {
            Sprite itemSprite = null;
            Image droppedImage = draggableItem.GetComponent<Image>();
            if (droppedImage != null) itemSprite = droppedImage.sprite;

            if (itemSprite == null)
            {
                SpriteRenderer droppedSr = draggableItem.GetComponent<SpriteRenderer>();
                if (droppedSr != null) itemSprite = droppedSr.sprite;
            }

            if (itemSprite != null)
            {
                string spriteName = itemSprite.name.ToLower();

                if (spriteName.Contains("bini"))
                {
                    ClearDroppedItem(draggableItem);
                    StartCoroutine(WashTempImageRoutine(
                        itemSprite, hatObject, biniScale, biniYOffset, biniRotationZ,
                        bini1EnterDuration, bini1WaitDuration, bini1ExitDuration,
                        bini2EnterDuration, bini2WaitDuration, bini2ExitDuration,
                        bini3EnterDuration, bini3WaitDuration,
                        bini1_2SinkDepth, bini3SinkDepth, biniRiseDuration, biniRiseHeight));
                }
                else if (spriteName.Contains("mophandle"))
                {
                    ClearDroppedItem(draggableItem);
                    StartCoroutine(WashTempImageRoutine(
                        itemSprite, blackMopHandleObject, mophandleScale, mophandleYOffset, mophandleRotationZ,
                        mophandle1EnterDuration, mophandle1WaitDuration, mophandle1ExitDuration,
                        mophandle2EnterDuration, mophandle2WaitDuration, mophandle2ExitDuration,
                        mophandle3EnterDuration, mophandle3WaitDuration,
                        mophandle1_2SinkDepth, mophandle3SinkDepth, mophandleRiseDuration, mophandleRiseHeight));
                }
            }
        }
    }

    private IEnumerator WashTempImageRoutine(
        Sprite itemSprite, GameObject resultObject, float targetScale, float targetYOffset, float targetRotZ,
        float e1, float w1, float x1,
        float e2, float w2, float x2,
        float e3, float w3,
        float normalSinkDep, float thirdSinkDep, float riseDur, float riseH)
    {
        isWashing = true;

        // 양동이 스프라이트 및 렌더러 가져오기 (레이어 설정용)
        SpriteRenderer bucketRenderer = GetComponent<SpriteRenderer>();
        if (bucketRenderer == null)
        {
            bucketRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // 1. 세척 연출용 월드 오브젝트 생성
        GameObject tempItem = new GameObject("TempWashItem");
        SpriteRenderer tempRenderer = tempItem.AddComponent<SpriteRenderer>();
        tempRenderer.sprite = itemSprite;

        if (bucketRenderer != null)
        {
            tempRenderer.sortingLayerID = bucketRenderer.sortingLayerID;
            tempRenderer.sortingOrder = bucketRenderer.sortingOrder - 2; // 양동이 뒤
        }
        else
        {
            tempRenderer.sortingOrder = -2;
        }

        // 3. 스케일 및 회전 적용
        tempItem.transform.localScale = new Vector3(targetScale, targetScale, 1f);
        tempItem.transform.rotation = Quaternion.Euler(0f, 0f, targetRotZ);

        // 4. 위치 계산
        Vector3 startPos = transform.position;
        startPos.y += targetYOffset;
        tempItem.transform.position = startPos;

        Vector3 normalSinkPos = startPos;
        normalSinkPos.y -= normalSinkDep;

        Vector3 thirdSinkPos = startPos;
        thirdSinkPos.y -= thirdSinkDep;

        // 6. [첫 번째] 들어가고(사운드+물튐) -> 대기 -> 나오고(사운드+물튐)
        PlaySound(wash1EnterSound);
        PlaySplash(startPos);
        yield return StartCoroutine(DoMoveMotion(tempItem, startPos, normalSinkPos, e1));
        
        yield return new WaitForSeconds(w1);
        
        PlaySound(wash1ExitSound);
        PlaySplash(startPos);
        yield return StartCoroutine(DoMoveMotion(tempItem, normalSinkPos, startPos, x1));

        // 7. [두 번째] 들어가고(사운드+물튐) -> 대기 -> 나오고(사운드+물튐)
        PlaySound(wash2EnterSound);
        PlaySplash(startPos);
        yield return StartCoroutine(DoMoveMotion(tempItem, startPos, normalSinkPos, e2));
        
        yield return new WaitForSeconds(w2);
        
        PlaySound(wash2ExitSound);
        PlaySplash(startPos);
        yield return StartCoroutine(DoMoveMotion(tempItem, normalSinkPos, startPos, x2));

        // 8. [세 번째] 깊이 들어가고(사운드+물튐) -> 대기 -> 소멸
        PlaySound(wash3EnterSound);
        PlaySplash(startPos);
        yield return StartCoroutine(DoMoveMotion(tempItem, startPos, thirdSinkPos, e3));
        
        yield return new WaitForSeconds(w3);
        Destroy(tempItem);

        // 9. 결과물 연출 시작 (양동이 뒤쪽에서 아래에서 위로 부드럽게 떠오름)
        if (resultObject != null)
        {
            SpriteRenderer resultSr = resultObject.GetComponent<SpriteRenderer>();
            if (resultSr != null && bucketRenderer != null)
            {
                resultSr.sortingLayerID = bucketRenderer.sortingLayerID;
                resultSr.sortingOrder = bucketRenderer.sortingOrder - 2;
            }

            Collider2D resultCol = resultObject.GetComponent<Collider2D>();
            if (resultCol != null) resultCol.enabled = false;

            Vector3 finalTargetPos = resultObject.transform.position;
            Vector3 spawnStartPos = finalTargetPos;
            spawnStartPos.y -= riseH;

            resultObject.transform.position = spawnStartPos;
            resultObject.SetActive(true);

            float elapsedTime = 0f;
            while (elapsedTime < riseDur)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / riseDur);
                float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

                resultObject.transform.position = Vector3.Lerp(spawnStartPos, finalTargetPos, smoothT);
                yield return null;
            }

            if (resultCol != null) resultCol.enabled = true;
        }

        isWashing = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }

    private void PlaySplash(Vector3 pos)
    {
        if (splashEffectPrefab != null)
        {
            GameObject splash = Instantiate(splashEffectPrefab, pos, Quaternion.identity);
            Destroy(splash, 2f); // 2초 후 자동 파괴 (이펙트 길이에 맞게 조절 가능)
        }
    }

    private IEnumerator DoMoveMotion(GameObject item, Vector3 fromPos, Vector3 toPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (item == null) yield break;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            item.transform.position = Vector3.Lerp(fromPos, toPos, t);
            yield return null;
        }
    }

    private void ClearDroppedItem(DraggableItemofStore draggable)
    {
        if (draggable != null)
        {
            draggable.wasDropped = true;
            draggable.ClearItem();
        }
    }
}