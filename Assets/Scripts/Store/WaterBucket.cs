using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterBucket : MonoBehaviour, IDropHandler
{
    [Header("bini 설정")]
    [SerializeField] private GameObject hatObject;
    [Tooltip("비니(bini) 씻길 때 크기 비율")]
    public float biniScale = 0.5f;
    [Tooltip("비니(bini) Y축 시작 위치 오프셋")]
    public float biniYOffset = 50.0f;

    [Header("mophandle 설정")]
    [SerializeField] private GameObject blackMopHandleObject;
    [Tooltip("자루(mophandle) 씻길 때 크기 비율")]
    public float mophandleScale = 0.8f; // 자루는 비니보다 더 길 수 있으므로 따로 설정
    [Tooltip("자루(mophandle) Y축 시작 위치 오프셋")]
    public float mophandleYOffset = 50.0f;

    [Header("사운드 설정")]
    [Tooltip("물에 씻는 효과음")]
    public AudioClip washSound;

    [Header("위아래 씻는 연출 공통 설정")]
    [Tooltip("아이템이 씻기는 총 시간(초)")]
    public float washDuration = 2.0f;

    [Tooltip("위아래로 움직이는 높이 폭")]
    public float moveDistance = 30.0f;

    [Tooltip("넣었다 뺐다 하는 속도")]
    public float moveSpeed = 8.0f;

    private bool isWashing = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (isWashing) return;

        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Image droppedImage = droppedObject.GetComponent<Image>();

            if (droppedImage != null && droppedImage.sprite != null)
            {
                Sprite itemSprite = droppedImage.sprite;
                string spriteName = itemSprite.name.ToLower();

                // 1. bini 처리
                if (spriteName.Contains("bini"))
                {
                    ClearDroppedItem(droppedObject);
                    // 🔍 bini 전용 크기(biniScale)와 오프셋(biniYOffset) 전달
                    StartCoroutine(WashTempImageRoutine(itemSprite, hatObject, biniScale, biniYOffset));
                }
                // 2. mophandle 처리
                else if (spriteName.Contains("mophandle"))
                {
                    ClearDroppedItem(droppedObject);
                    // 🔍 mophandle 전용 크기(mophandleScale)와 오프셋(mophandleYOffset) 전달
                    StartCoroutine(WashTempImageRoutine(itemSprite, blackMopHandleObject, mophandleScale, mophandleYOffset));
                }
            }
        }
    }

    // 🌊 개별 scale과 yOffset을 인자로 받아 처리하는 코루틴
    private IEnumerator WashTempImageRoutine(Sprite itemSprite, GameObject resultObject, float targetScale, float targetYOffset)
    {
        isWashing = true;

        // 🔊 1. 씻는 소리 재생
        if (washSound != null)
        {
            AudioSource.PlayClipAtPoint(washSound, Camera.main.transform.position);
        }

        // 🖼️ 2. 임시 UI 오브젝트 생성
        GameObject tempItem = new GameObject("TempWashItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        tempItem.transform.SetParent(transform.parent, false);
        tempItem.transform.SetAsLastSibling();

        Image tempImage = tempItem.GetComponent<Image>();
        tempImage.sprite = itemSprite;
        tempImage.SetNativeSize();

        // 🔍 3. 전달받은 아이템별 크기 적용
        tempItem.transform.localScale = new Vector3(targetScale, targetScale, 1f);

        // 📍 4. 전달받은 아이템별 Y 오프셋 위치 적용
        tempItem.transform.position = transform.position;
        Vector3 basePosition = tempItem.transform.localPosition;
        basePosition.y += targetYOffset;

        float elapsedTime = 0f;

        // 💦 5. 위아래 퐁당퐁당 연출
        while (elapsedTime < washDuration)
        {
            elapsedTime += Time.deltaTime;

            float offsetY = Mathf.Sin(elapsedTime * moveSpeed) * moveDistance;
            tempItem.transform.localPosition = new Vector3(basePosition.x, basePosition.y + offsetY, basePosition.z);

            yield return null;
        }

        // 🗑️ 6. 임시 오브젝트 삭제
        Destroy(tempItem);

        // ✨ 7. 결과물 등장
        if (resultObject != null)
        {
            resultObject.SetActive(true);
        }

        isWashing = false;
    }

    private void ClearDroppedItem(GameObject droppedObject)
    {
        DraggableItemofStore draggable = droppedObject.GetComponent<DraggableItemofStore>();
        if (draggable != null)
        {
            draggable.wasDropped = true;
            draggable.ClearItem();
        }
    }
}