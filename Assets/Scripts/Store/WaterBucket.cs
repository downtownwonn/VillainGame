using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterBucket : MonoBehaviour, IDropHandler
{
    [Header("bini를 넣었을 때 활성화할 Hat 오브젝트")]
    [SerializeField] private GameObject hatObject; 

    [Header("mophandle을 넣었을 때 활성화할 Black Mop Handle 오브젝트")]
    [SerializeField] private GameObject blackMopHandleObject;

    [Header("사운드 설정")]
    [Tooltip("물에 씻는 효과음")]
    public AudioClip washSound;

    [Header("연출 설정")]
    [Tooltip("아이템이 씻기는 시간(초)")]
    public float washDuration = 1.0f;
    [Tooltip("아이템 흔들림 강도")]
    public float shakeMagnitude = 15.0f;

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
                    // 원래 슬롯은 깔끔하게 즉시 비움
                    ClearDroppedItem(droppedObject);

                    // 임시 이미지를 만들어 씻는 연출 진행
                    StartCoroutine(WashTempImageRoutine(itemSprite, hatObject));
                }
                // 2. mophandle 처리
                else if (spriteName.Contains("mophandle"))
                {
                    ClearDroppedItem(droppedObject);

                    StartCoroutine(WashTempImageRoutine(itemSprite, blackMopHandleObject));
                }
            }
        }
    }

    // 🌊 임시 이미지 생성 -> 흔들기 -> 삭제 -> 결과물 등장 코루틴
    private IEnumerator WashTempImageRoutine(Sprite itemSprite, GameObject resultObject)
    {
        isWashing = true;

        // 🔊 1. 씻는 소리 재생
        if (washSound != null)
        {
            AudioSource.PlayClipAtPoint(washSound, Camera.main.transform.position);
        }

        // 🖼️ 2. 흔들기용 임시 UI 오브젝트 즉석 생성
        GameObject tempItem = new GameObject("TempWashItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        
        // 물통의 부모(Canvas 등) 아래로 넣어주어 화면 맨 위에 보이도록 설정
        tempItem.transform.SetParent(transform.parent, false);
        tempItem.transform.SetAsLastSibling(); // 맨 앞으로 올리기

        // 이미지 복사 및 크기 맞추기
        Image tempImage = tempItem.GetComponent<Image>();
        tempImage.sprite = itemSprite;
        tempImage.SetNativeSize(); // 원래 이미지 비율 유지

        // 위치를 물통 중앙으로 지정
        tempItem.transform.position = transform.position;
        Vector3 basePosition = tempItem.transform.localPosition;

        float elapsedTime = 0f;

        // 💦 3. 임시 이미지 흔들기
        while (elapsedTime < washDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            tempItem.transform.localPosition = basePosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 🗑️ 4. 다 씻었으니 임시 이미지 완전 삭제
        Destroy(tempItem);

        // ✨ 5. 결과물(Hat / Black Mop Handle) 짜잔 나타남
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