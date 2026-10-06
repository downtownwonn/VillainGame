using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundSwapDropZone : MonoBehaviour, IDropHandler
{
    public SpriteRenderer backgroundRenderer; // 남자머리의 Sprite Renderer
    public Sprite newBackgroundSprite;        // 금발 장발 이미지

    [Header("가발로 바뀔 때 적용할 위치/크기")]
    public Vector3 wigLocalPosition = new Vector3(-2.1f, 0.67f, 0f);
    public Vector3 wigLocalScale = new Vector3(0.25f, 0.25f, 1f);

    [Header("효과음")]
    public AudioClip swapSound;
    public AudioSource audioSource;

    [Header("클리어 씬 이동")]
    public string clearSceneName = "Library R2 Clear"; // Scene List에 넣은 씬 이름과 똑같이
    public float delayBeforeClear = 1.5f;              // 가발 쓴 모습 보여줄 시간(초)

    [Header("가발 판정 (비워두면 아무 아이템이나 통과)")]
    public Sprite wigIconSprite; // 인벤토리에 뜨는 가발 아이콘 이미지

    private bool isCleared = false; // 중복 실행 방지
    private Image zoneImage;

    void Awake()
    {
        zoneImage = GetComponent<Image>();
    }

    void Update()
    {
        // 마우스를 누르고 있는 동안(드래그 중)과 뗀 순간에만 드롭을 받음
        // 처음 누르는 순간엔 꺼둬서 캐릭터 클릭(말풍선)을 막지 않음
        if (zoneImage == null) return;

        bool active = (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                      && !Input.GetMouseButtonDown(0);
        zoneImage.raycastTarget = active;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨!");

        if (isCleared) return;

        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 가발이 아니면 무시 (wigIconSprite를 넣었을 때만 검사)
        if (wigIconSprite != null && droppedImage.sprite != wigIconSprite)
        {
            Debug.Log("가발이 아님: " + droppedImage.sprite.name);
            if (draggable != null) draggable.ReturnToOriginalPosition();
            return;
        }

        isCleared = true;

        // 이미지 교체
        backgroundRenderer.sprite = newBackgroundSprite;

        // 가발 전용 위치/크기 적용
        backgroundRenderer.transform.localPosition = wigLocalPosition;
        backgroundRenderer.transform.localScale = wigLocalScale;

        // 효과음 재생
        if (audioSource != null && swapSound != null)
        {
            audioSource.PlayOneShot(swapSound);
        }

        // 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color c = droppedImage.color;
        c.a = 0f;
        droppedImage.color = c;

        if (draggable != null)
        {
            draggable.ReturnToOriginalPosition();
        }

        Debug.Log("배경 교체 완료!");

        // 일정 시간 후 클리어 씬으로 이동
        StartCoroutine(GoToClearScene());
    }

    private IEnumerator GoToClearScene()
    {
        yield return new WaitForSeconds(delayBeforeClear);
        Debug.Log("클리어 씬으로 이동: " + clearSceneName);
        SceneManager.LoadScene(clearSceneName);
    }
}