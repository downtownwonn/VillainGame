using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
            // UI Image 또는 SpriteRenderer에서 스프라이트 추출
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

        // 1. 세척 연출용 월드 오브젝트 생성 (월드 공간 렌더링)
        GameObject tempItem = new GameObject("TempWashItem");
        SpriteRenderer tempRenderer = tempItem.AddComponent<SpriteRenderer>();
        tempRenderer.sprite = itemSprite;

        // 2. 양동이 SpriteRenderer 가져오기 및 Sorting Order 설정 (양동이 뒤로 가도록 -1)
        SpriteRenderer bucketRenderer = GetComponent<SpriteRenderer>();
        if (bucketRenderer == null)
        {
            bucketRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (bucketRenderer != null)
        {
            tempRenderer.sortingLayerID = bucketRenderer.sortingLayerID;
            tempRenderer.sortingOrder = bucketRenderer.sortingOrder - 1; // 양동이 바로 뒤
        }
        else
        {
            tempRenderer.sortingOrder = -1;
        }

        // 3. 스케일 적용
        tempItem.transform.localScale = new Vector3(targetScale, targetScale, 1f);

        // 4. 양동이 월드 위치 기준으로 시작 위치 및 입수 위치 설정
        Vector3 startPos = transform.position;
        startPos.y += targetYOffset;
        tempItem.transform.position = startPos;

        Vector3 sinkPos = startPos;
        sinkPos.y -= sinkDepth;

        // 5. 효과음 재생
        if (washSound != null)
        {
            AudioSource.PlayClipAtPoint(washSound, Camera.main.transform.position);
        }

        // 6. [입수 모션] 양동이 물속으로 내려감
        float elapsedTime = 0f;
        while (elapsedTime < enterDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / enterDuration);
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            tempItem.transform.position = Vector3.Lerp(startPos, sinkPos, smoothT);
            yield return null;
        }

        // 7. [세척 대기]
        yield return new WaitForSeconds(washDuration);

        // 8. [출수 모션] 다시 위로 올라옴
        elapsedTime = 0f;
        while (elapsedTime < exitDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / exitDuration);
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            tempItem.transform.position = Vector3.Lerp(sinkPos, startPos, smoothT);
            yield return null;
        }

        // 9. 임시 비니/자루 삭제
        Destroy(tempItem);

        // 10. 결과물(hat 또는 bong) 월드 오브젝트 활성화
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