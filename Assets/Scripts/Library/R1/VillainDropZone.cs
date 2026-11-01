using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VillainDropZone : MonoBehaviour, IDropHandler
{
    public Transform stackParent;              // 책이 쌓일 위치 (부모)
    public GameObject stackedBookPrefab;       // 쌓인 책 하나를 표현할 이미지 프리팹
    public float stackHeight = 15f;            // 책 한 권마다 올라갈 높이
    public int totalBooksNeeded = 5;           // 몇 권 모으면 퇴치되는지

    private int stackedCount = 0;
    private bool isCleared = false;

    [Header("빌런 다리 애니메이션")]
    public Animator legAnimator;               // 다리 떠는 애니메이터 연결
    public SpriteRenderer legSprite;           // 다리 SpriteRenderer 연결
    public Sprite idleLegSprite;               // 안 떠는 기본 자세 프레임

    [Header("효과음")]
    public AudioClip stackSound;    // 책 한 권 쌓일 때마다 재생할 효과음
    public AudioSource audioSource; // AudioManager의 AudioSource 연결

    [Header("클리어 씬 이동 (비워두면 이동 안 함)")]
    public string clearSceneName = "";
    public float delayBeforeClear = 1.5f;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨! pointerDrag = " + eventData.pointerDrag);

        if (isCleared) return;

        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 쌓인 책 이미지 생성
        GameObject newBook = Instantiate(stackedBookPrefab, stackParent);
        newBook.GetComponent<Image>().sprite = droppedImage.sprite;

        RectTransform rt = newBook.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, stackedCount * stackHeight);

        stackedCount++;

        // 책 쌓이는 효과음 재생
        if (audioSource != null && stackSound != null)
        {
            audioSource.PlayOneShot(stackSound);
        }

        // 원래 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color c = droppedImage.color;
        c.a = 0f;
        droppedImage.color = c;

        // 드래그했던 아이템 원래 자리로
        if (draggable != null)
        {
            draggable.ReturnToOriginalPosition();
        }

        if (stackedCount >= totalBooksNeeded)
        {
            Debug.Log("빌런 퇴치 성공!");
            isCleared = true;

            // 다리 애니메이션 정지
            if (legAnimator != null)
            {
                legAnimator.enabled = false;
            }
            if (legSprite != null && idleLegSprite != null)
            {
                legSprite.sprite = idleLegSprite;
            }

            if (!string.IsNullOrEmpty(clearSceneName))
                StartCoroutine(GoToClearScene());
        }
    }

    private IEnumerator GoToClearScene()
    {
        yield return new WaitForSeconds(delayBeforeClear);
        Debug.Log("클리어 씬으로 이동: " + clearSceneName);
        SceneManager.LoadScene(clearSceneName);
    }
}