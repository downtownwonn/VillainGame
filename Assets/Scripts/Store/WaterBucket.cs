using System.Collections;
using UnityEngine;

public class WaterBucket : MonoBehaviour
{
    [Header("bini 설정")]
    [SerializeField] private GameObject hatObject;
    [Tooltip("비니 씻길 때 크기 비율")]
    public float biniScale = 0.5f;
    [Tooltip("비니 Y축 시작 위치 오프셋 (월드 단위, 예: 0.5)")]
    public float biniYOffset = 0.5f;

    [Header("mophandle 설정")]
    [SerializeField] private GameObject blackMopHandleObject;
    [Tooltip("자루 씻길 때 크기 비율")]
    public float mophandleScale = 0.8f; 
    [Tooltip("자루 Y축 시작 위치 오프셋 (월드 단위, 예: 0.5)")]
    public float mophandleYOffset = 0.5f;

    [Header("사운드 설정")]
    [Tooltip("물에 씻는 효과음")]
    public AudioClip washSound;

    [Header("씻는 연출 타이밍 설정")]
    [Tooltip("아이템이 물속으로 쑥 들어가는 시간(초)")]
    public float enterDuration = 0.5f;

    [Tooltip("물속에서 씻겨지는(유지되는) 시간(초)")]
    public float washDuration = 1.0f;

    [Tooltip("아이템이 쑥 빠져나오는 시간(초)")]
    public float exitDuration = 0.5f;

    [Tooltip("들어갈 때 아래로 내려가는 거리 (월드 단위, 예: 0.5)")]
    public float sinkDepth = 0.5f;

    private bool isWashing = false;

    public void OnItemDropped(DraggableItemofStore draggableItem)
    {
        if (isWashing) return;

        if (draggableItem != null)
        {
            // 드롭된 아이템에서 스프라이트 추출 (UI Image 또는 SpriteRenderer 모두 대응)
            Sprite itemSprite = null;
            var droppedImage = draggableItem.GetComponent<UnityEngine.UI.Image>();
            if (droppedImage != null) itemSprite = droppedImage.sprite;

            if (itemSprite == null)
            {
                var droppedSr = draggableItem.GetComponent<SpriteRenderer>();
                if (droppedSr != null) itemSprite = droppedSr.sprite;
            }

            if (itemSprite != null)
            {
                string spriteName = itemSprite.name.ToLower();

                if (spriteName.Contains("bini"))
                {
                    ClearDroppedItem(draggableItem);
                    StartCoroutine(WashTempImageRoutine(itemSprite, hatObject, biniScale, biniYOffset));
                }
                else if (spriteName.Contains("mophandle"))
                {
                    ClearDroppedItem(draggableItem);
                    StartCoroutine(WashTempImageRoutine(itemSprite, blackMopHandleObject, mophandleScale, mophandleYOffset));
                }
            }
        }
    }

    private IEnumerator WashTempImageRoutine(Sprite itemSprite, GameObject resultObject, float targetScale, float targetYOffset)
    {
        isWashing = true;

        // 1. 새로운 월드 오브젝트 생성 및 SpriteRenderer 부착
        GameObject tempItem = new GameObject("TempWashItem");
        SpriteRenderer tempRenderer = tempItem.AddComponent<SpriteRenderer>();
        tempRenderer.sprite = itemSprite;

        // 🌊 [핵심 레이어 처리] 
        // WaterBucket 자신 또는 자식 오브젝트에서 양동이의 SpriteRenderer를 안전하게 찾음
        SpriteRenderer bucketRenderer = GetComponent<SpriteRenderer>();
        if (bucketRenderer == null)
        {
            bucketRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (bucketRenderer != null)
        {
            // 양동이와 같은 레이어를 쓰되, 순서만 정확히 1칸 뒤로 설정 (-1)
            tempRenderer.sortingLayerID = bucketRenderer.sortingLayerID;
            tempRenderer.sortingOrder = bucketRenderer.sortingOrder - 1;
        }
        else
        {
            // 양동이에 렌더러가 아예 없다면 기본값으로 지정
            tempRenderer.sortingOrder = -1;
        }

        // 크기 적용
        tempItem.transform.localScale = new Vector3(targetScale, targetScale, 1f);

        // 2. 월드 좌표 기준으로 위치 설정
        Vector3 startPos = transform.position;
        startPos.y += targetYOffset;
        tempItem.transform.position = startPos;

        Vector3 sinkPos = startPos;
        sinkPos.y -= sinkDepth; // 물속으로 들어간 월드 위치

        float elapsedTime = 0f;

        // 🔊 3. 씻는 소리 재생
        if (washSound != null)
        {
            AudioSource.PlayClipAtPoint(washSound, Camera.main.transform.position);
        }

        // 4. [입수 모션] 물속으로 쑥 들어감
        while (elapsedTime < enterDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / enterDuration);
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            tempItem.transform.position = Vector3.Lerp(startPos, sinkPos, smoothT);
            yield return null;
        }

        // 5. [세척 대기] 물속에서 머무는 시간
        yield return new WaitForSeconds(washDuration);

        // 6. [출수 모션] 물속에서 다시 위로 쑥 빠져나옴
        elapsedTime = 0f;
        while (elapsedTime < exitDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / exitDuration);
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            tempItem.transform.position = Vector3.Lerp(sinkPos, startPos, smoothT);
            yield return null;
        }

        // 7. 임시 오브젝트 제거
        Destroy(tempItem);

        // 8. 결과물 등장
        if (resultObject != null)
        {
            resultObject.SetActive(true);
        }

        isWashing = false;
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