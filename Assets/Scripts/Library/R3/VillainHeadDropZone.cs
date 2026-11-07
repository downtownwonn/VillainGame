using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VillainHeadDropZone : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Image headSpeakerImage;   // 빌런 머리 위에 나타날 이미지
    public Sprite muteSprite;        // 클릭하면 바뀔 무음 스피커 이미지

    public SpriteRenderer backgroundRenderer; // 바뀔 배경의 Sprite Renderer
    public Sprite newBackgroundSprite;        // 바뀐 후의 배경 이미지

    [Header("배경음악")]
    public AudioSource bgmSource;         // 기존 배경음악
    public AudioSource extraMusicSource;  // 추가로 틀어둔 내 음악

    [Header("클리어 씬 이동 (비워두면 이동 안 함)")]
    public string clearSceneName = "";
    public float delayBeforeClear = 1.5f;
    [SerializeField] private float fadeDuration = 1f;

    private bool isPlaced = false;   // 스피커가 놓였는지 여부
    private bool isCleared = false;  // 중복 실행 방지

    private void Awake()
    {
        // 머리 위 스피커 이미지는 표시만 담당한다. 레이캐스트를 막으면
        // 아래 DropZone으로 클릭 이벤트가 전달되지 않을 수 있다.
        if (headSpeakerImage != null)
            headSpeakerImage.raycastTarget = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        headSpeakerImage.sprite = droppedImage.sprite;
        Color c = headSpeakerImage.color;
        c.a = 1f;
        headSpeakerImage.color = c;

        isPlaced = true;

        droppedImage.sprite = null;
        Color slotColor = droppedImage.color;
        slotColor.a = 0f;
        droppedImage.color = slotColor;

        Destroy(droppedItem);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCleared) return;

        if (isPlaced && muteSprite != null && headSpeakerImage != null)
        {
            isCleared = true;

            headSpeakerImage.sprite = muteSprite;
            Debug.Log("무음으로 변경됨!");

            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }

            // 배경음악 전부 정지
            if (bgmSource != null) bgmSource.Stop();
            if (extraMusicSource != null) extraMusicSource.Stop();

            // 클리어 연출 후 검은 화면으로 페이드아웃하고 씬을 전환한다.
            if (!string.IsNullOrEmpty(clearSceneName))
                StartCoroutine(GoToClearScene());
        }
    }

    private IEnumerator GoToClearScene()
    {
        yield return new WaitForSeconds(delayBeforeClear);
        Debug.Log("클리어 씬으로 이동: " + clearSceneName);

        Image fadeImage = CreateFadeOverlay();
        Color color = Color.black;
        color.a = 0f;
        fadeImage.color = color;

        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
        SceneManager.LoadScene(clearSceneName);
    }

    private Image CreateFadeOverlay()
    {
        GameObject overlay = new GameObject("ClearFadeOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(Image));
        Canvas canvas = overlay.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32767;

        RectTransform rect = overlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = overlay.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;
        return image;
    }
}
