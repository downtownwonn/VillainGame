using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubwayBubbleManager : MonoBehaviour
{
    public static SubwayBubbleManager Instance { get; private set; }

    [Header("Bubble")]
    [SerializeField] private GameObject bubbleObject;
    [SerializeField] private Image bubbleImage;
    [SerializeField] private TMP_Text bubbleText;

    [Header("Bubble Sprites")]
    [SerializeField] private Sprite[] bubbleSprites;

    [Header("Bubble Canvas")]
    [SerializeField] private Canvas canvas;

    [Header("Bubble Default Sizes")]
    [SerializeField]
    private Vector2[] bubbleSizes =
    {
        new Vector2(250f, 100f),   // 0
        new Vector2(300f, 120f),   // 1
        new Vector2(350f, 140f),   // 2
        new Vector2(400f, 160f)    // 3
    };

    [Header("Bubble Type Offsets")]
    [SerializeField]
    private Vector2[] bubbleOffsets =
    {
        new Vector2(0f, 0f),       // 0
        new Vector2(0f, 0f),       // 1
        new Vector2(0f, 0f),       // 2
        new Vector2(0f, 0f),       // 3
        new Vector2(-7f, 0f),      // 4
        new Vector2(-7f, 0f),      // 5
        new Vector2(-7f, 0f),      // 6
        new Vector2(-7f, 0f)       // 7
    };

    [Header("Text Offsets")]
    [SerializeField]
    private Vector2[] textOffsets =
    {
        new Vector2(0f, 0f),       // 0
        new Vector2(0f, 0f),       // 1
        new Vector2(0f, 0f),       // 2
        new Vector2(0f, 0f),       // 3
        new Vector2(0f, 0f),       // 4
        new Vector2(0f, 0f),       // 5
        new Vector2(0f, 0f),       // 6
        new Vector2(0f, 0f)        // 7
    };

    [Header("Settings")]
    [SerializeField] private float displayDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private bool isShowing = false;
    private Coroutine hideCoroutine;

    public bool IsShowing => isShowing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }
    }

    public void ShowBubble(SubwayBubbleCharacter character)
    {
        if (isShowing)
        {
            return;
        }

        if (character == null)
        {
            return;
        }

        isShowing = true;

        int bubbleType = character.BubbleType;

        // =========================================
        // 말풍선 이미지 설정
        // =========================================

        if (bubbleImage != null &&
            bubbleSprites != null &&
            bubbleSprites.Length > 0)
        {
            int spriteIndex = bubbleType % 4;

            if (spriteIndex >= 0 &&
                spriteIndex < bubbleSprites.Length)
            {
                bubbleImage.sprite = bubbleSprites[spriteIndex];
            }

            // 0~3 = 기본
            // 4~7 = 좌우 반전
            if (bubbleType >= 4)
            {
                bubbleImage.rectTransform.localScale =
                    new Vector3(-1f, 1f, 1f);
            }
            else
            {
                bubbleImage.rectTransform.localScale =
                    new Vector3(1f, 1f, 1f);
            }

            bubbleImage.preserveAspect = true;

            int sizeIndex = bubbleType % 4;

            if (sizeIndex >= 0 &&
                sizeIndex < bubbleSizes.Length)
            {
                bubbleImage.rectTransform.sizeDelta =
                    bubbleSizes[sizeIndex];
            }
        }

        // =========================================
        // 텍스트 설정
        // =========================================

        if (bubbleText != null)
        {
            bubbleText.text = character.Dialogue;

            Debug.Log("대사: [" + character.Dialogue + "]");

            bubbleText.rectTransform.localScale =
                new Vector3(1f, 1f, 1f);

            if (bubbleType >= 0 &&
                bubbleType < textOffsets.Length)
            {
                Vector2 managerTextOffset =
                    textOffsets[bubbleType];

                Vector2 characterTextOffset =
                    character.TextOffset;

                bubbleText.rectTransform.anchoredPosition =
                    managerTextOffset + characterTextOffset;
            }
        }

        // =========================================
        // 말풍선 위치 설정
        // =========================================

        SetBubblePosition(character);

        // =========================================
        // 말풍선 표시
        // =========================================

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(true);
        }

        // =========================================
        // 사운드
        // =========================================

        if (audioSource != null &&
            character.Sound != null)
        {
            audioSource.PlayOneShot(character.Sound);
        }

        // =========================================
        // 자동 숨김
        // =========================================

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideBubbleAfterDelay());
    }

    private void SetBubblePosition(SubwayBubbleCharacter character)
    {
        if (bubbleObject == null ||
            canvas == null)
        {
            return;
        }

        RectTransform bubbleRect =
            bubbleObject.GetComponent<RectTransform>();

        if (bubbleRect == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        int bubbleType = character.BubbleType;

        if (bubbleType < 0 ||
            bubbleType >= bubbleOffsets.Length)
        {
            return;
        }

        Vector2 managerOffset =
            bubbleOffsets[bubbleType];

        Vector2 characterOffset =
            character.BubbleOffset;

        Vector3 offset = new Vector3(
            managerOffset.x + characterOffset.x,
            managerOffset.y + characterOffset.y,
            0f
        );

        Vector3 worldPosition =
            character.transform.position + offset;

        Vector2 screenPosition =
            mainCamera.WorldToScreenPoint(worldPosition);

        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();

        Camera canvasCamera = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = canvas.worldCamera;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvasCamera,
            out Vector2 localPosition
        );

        bubbleRect.localPosition = localPosition;
    }

    public void HideBubble()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }

        isShowing = false;
    }

    private IEnumerator HideBubbleAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }

        isShowing = false;
        hideCoroutine = null;
    }
}