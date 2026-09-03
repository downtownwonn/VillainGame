using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }

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

    public void ShowBubble(BubbleCharacter character)
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

            // 0~3 = 오른쪽
            // 4~7 = 왼쪽
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

            // Bubble Type에 따른 크기
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

            // 텍스트는 좌우반전되지 않도록 유지
            bubbleText.rectTransform.localScale =
                new Vector3(1f, 1f, 1f);

            // Bubble Type별 텍스트 위치
            if (bubbleType >= 0 &&
    bubbleType < textOffsets.Length)
            {
                Vector2 managerTextOffset = textOffsets[bubbleType];
                Vector2 characterTextOffset = character.TextOffset;

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
        // 기존 숨김 코루틴 정리
        // =========================================

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideBubbleAfterDelay());
    }

    private void SetBubblePosition(BubbleCharacter character)
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

        // BubbleManager의 기본 Offset
        Vector2 managerOffset =
            bubbleOffsets[bubbleType];

        // BubbleCharacter의 개별 Offset
        Vector2 characterOffset =
            character.BubbleOffset;

        // 두 Offset을 합침
        Vector3 offset = new Vector3(
            managerOffset.x + characterOffset.x,
            managerOffset.y + characterOffset.y,
            0f
        );

        // 캐릭터 위치 + 최종 Offset
        Vector3 worldPosition =
            character.transform.position + offset;

        // 월드 좌표 -> 스크린 좌표
        Vector2 screenPosition =
            mainCamera.WorldToScreenPoint(worldPosition);

        // Canvas 좌표로 변환
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
