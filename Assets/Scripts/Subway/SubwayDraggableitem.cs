using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubwayDraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas canvas;

    [HideInInspector]
    public bool wasDropped = false;

    public Action<PointerEventData> OnDragEnded;

    [Header("Scissors Item Data")]
    [SerializeField] private ItemData scissorsItemData;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvas = GetComponentInParent<Canvas>();
    }


    // =========================================================
    // 드래그 시작
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();

        if (img == null || img.sprite == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        wasDropped = false;

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }


    // =========================================================
    // 드래그 중
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }


    // =========================================================
    // 드래그 종료
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        InventoryItemData draggedItemData =
            GetComponent<InventoryItemData>();


        // =====================================================
        // R3
        // 경호원 3인조 → 자르반 할아버지
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Bodyguards)
        {
            Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(eventData.position);

            worldPosition.z = 0f;

            Collider2D hit =
                Physics2D.OverlapPoint(worldPosition);

            if (hit != null)
            {
                JarvanVillain villain =
                    hit.GetComponent<JarvanVillain>();

                if (villain == null)
                {
                    villain =
                        hit.GetComponentInParent<JarvanVillain>();
                }

                if (villain != null)
                {
                    Image bodyguardImage =
                        GetComponent<Image>();

                    bool used =
                        villain.UseBodyguards(bodyguardImage);

                    if (used)
                    {
                        ReturnToOriginalPosition();
                        StartCoroutine(ClearAndFade());
                        return;
                    }
                }
            }
        }


        // =====================================================
        // R3
        // 선글라스 → 직장인
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Sunglasses)
        {
            Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(eventData.position);

            worldPosition.z = 0f;

            Collider2D[] hits =
                Physics2D.OverlapPointAll(worldPosition);

            foreach (Collider2D hit in hits)
            {
                BodyguardCharacter bodyguard =
                    hit.GetComponent<BodyguardCharacter>();

                if (bodyguard == null)
                {
                    bodyguard =
                        hit.GetComponentInParent<BodyguardCharacter>();
                }

                if (bodyguard != null)
                {
                    Debug.Log("선글라스를 직장인에게 사용!");

                    // 선글라스 착용 성공 여부 확인
                    bool used =
                        bodyguard.PutOnSunglasses();

                    // 이미 선글라스를 쓴 직장인이라면 사용하지 않음
                    if (!used)
                    {
                        Debug.Log(
                            "이미 선글라스를 착용한 직장원입니다."
                        );

                        ReturnToOriginalPosition();
                        return;
                    }

                    // 착용 성공했을 때만 선글라스 제거
                    Image sunglassesImage =
                        GetComponent<Image>();

                    if (sunglassesImage != null)
                    {
                        sunglassesImage.sprite = null;
                        sunglassesImage.enabled = false;
                    }

                    ReturnToOriginalPosition();

                    return;
                }
            }
        }


       // =====================================================
       // R2
       // 상모 → 장구 부원
       // =====================================================

       if (draggedItemData != null &&
           draggedItemData.itemType == ItemType.Sangmo)
       {
           Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(eventData.position);

           worldPosition.z = 0f;

           Collider2D[] hits =
                Physics2D.OverlapPointAll(worldPosition);

           Debug.Log("[R2] 상모 드롭 감지! 충돌 개수: " + hits.Length);

           foreach (Collider2D hit in hits)
           {
                JangguCharacter jangguCharacter =
                    hit.GetComponent<JangguCharacter>();

                if (jangguCharacter == null)
                {
                    jangguCharacter =
                        hit.GetComponentInParent<JangguCharacter>();
                }

                if (jangguCharacter == null)
                {
                    jangguCharacter =
                        hit.GetComponentInChildren<JangguCharacter>();
                }

                if (jangguCharacter != null)
                {
                    Debug.Log("[R2] 장구 부원 발견!");

                    jangguCharacter.ReceiveSangmo();

                    Image usedItemImage =
                        GetComponent<Image>();

                if (usedItemImage != null)
                {
                    usedItemImage.sprite = null;
                    usedItemImage.enabled = false;
                }

                ReturnToOriginalPosition();

                Debug.Log("[R2] 상모 착용 성공!");

                return;
                }
            }

            Debug.LogWarning("[R2] 장구 부원을 찾지 못했습니다.");
        }


        
// =====================================================
// R2
// 장구 → 메인 배경 주인공 정자세
// =====================================================

if (draggedItemData != null &&
    draggedItemData.itemType == ItemType.Janggu)
{
    Vector3 worldPosition =
        Camera.main.ScreenToWorldPoint(eventData.position);

    worldPosition.z = 0f;

    Collider2D[] hits =
        Physics2D.OverlapPointAll(worldPosition);

    Debug.Log("[R2] 장구 드롭 감지! 충돌 개수: " + hits.Length);

    foreach (Collider2D hit in hits)
    {
        Debug.Log("[R2] 충돌 오브젝트: " + hit.name);

        DansoVillain villain =
            hit.GetComponent<DansoVillain>();

        if (villain == null)
        {
            villain =
                hit.GetComponentInParent<DansoVillain>();
        }

        if (villain == null)
        {
            villain =
                hit.GetComponentInChildren<DansoVillain>();
        }

        if (villain != null && villain.isR2ClearTarget)
        {
            Debug.Log("[R2] 주인공 정자세 드롭 대상 발견!");

            villain.ReceiveJanggu();

            Image usedItemImage =
                GetComponent<Image>();

            if (usedItemImage != null)
            {
                usedItemImage.sprite = null;
                usedItemImage.enabled = false;
            }

            ReturnToOriginalPosition();

            StartCoroutine(ClearAndFade());

            Debug.Log("[R2] 장구 전달 성공!");

            return;
        }
    }

    Debug.LogWarning("[R2] 주인공 정자세를 찾지 못했습니다. 장구를 원래 위치로 돌립니다.");
}



        // =====================================================
        // R1
        // 가위 → 백팩 빌런
        // =====================================================

        if (draggedItemData != null &&
            draggedItemData.itemType == ItemType.Scissors)
        {
            Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(eventData.position);

            worldPosition.z = 0f;

            Collider2D hit =
                Physics2D.OverlapPoint(worldPosition);

            if (hit != null)
            {
                BackpackVillainTarget backpack =
                    hit.GetComponent<BackpackVillainTarget>();

                if (backpack == null)
                {
                    backpack =
                        hit.GetComponentInParent<BackpackVillainTarget>();
                }

                if (backpack != null)
                {
                    Image scissorsImage =
                        GetComponent<Image>();

                    bool used =
                        backpack.UseScissors(scissorsImage);

                    if (used)
                    {
                        Debug.Log("가위 → 백팩 빌런 성공!");

                        ReturnToOriginalPosition();

                        StartCoroutine(ClearAndFade());

                        return;
                    }
                }
            }
        }


        // =====================================================
        // R1
        // 인벤토리 아이템끼리 합치기
        // =====================================================

        Image targetImage = null;

        foreach (GameObject obj in eventData.hovered)
        {
            Image image =
                obj.GetComponent<Image>();

            if (image == null)
                continue;

            if (image == GetComponent<Image>())
                continue;

            if (image.sprite == null)
                continue;

            InventoryItemData data =
                image.GetComponent<InventoryItemData>();

            if (data != null)
            {
                targetImage = image;
                break;
            }
        }


        // =====================================================
        // R1
        // 손잡이 + 손잡이 = 가위
        // =====================================================

        if (targetImage != null)
        {
            bool combined =
                SubwayInventoryManager.Instance.TryCombine(
                    GetComponent<Image>(),
                    targetImage,
                    scissorsItemData
                );

            if (combined)
            {
                return;
            }
        }


        // =====================================================
        // 아무것도 성공하지 않았으면 원래 자리로
        // =====================================================

        ReturnToOriginalPosition();

        if (OnDragEnded != null)
        {
            OnDragEnded.Invoke(eventData);
        }
    }


    // =========================================================
    // 원래 인벤토리 슬롯으로 복귀
    // =========================================================

    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }

        rectTransform.anchoredPosition = originalPosition;
    }


    // =========================================================
    // 인벤토리 아이템 클릭
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();

        // 빈 슬롯이면 아무 반응 없음
        if (img == null || img.sprite == null)
            return;

        // 새 아이템 설명 팝업 시스템 사용
        ItemInfoTrigger trigger =
            GetComponent<ItemInfoTrigger>();

        if (trigger != null)
        {
            trigger.OnPointerClick(eventData);
        }
    }
    
private IEnumerator ClearAndFade()
{
    yield return new WaitForSeconds(0.2f);

    string currentScene =
        SceneManager.GetActiveScene().name;

    string clearSceneName = null;

    // R1 클리어
    if (currentScene == "SubwayR1Play")
    {
        clearSceneName = "SubwayR1Clear";
    }
    // R2 클리어
    else if (currentScene == "SubwayR2Play")
    {
        clearSceneName = "SubwayR2Clear";
    }
    // R3 클리어
    else if (currentScene == "SubwayR3Play")
    {
        clearSceneName = "SubwayR3Clear";
    }

    if (string.IsNullOrEmpty(clearSceneName))
    {
        Debug.LogError("클리어 씬을 찾지 못했습니다: " + currentScene);
        yield break;
    }

    // 클리어 씬 등록 여부 확인
    if (!Application.CanStreamedLevelBeLoaded(clearSceneName))
    {
        Debug.LogError("Build Profiles에 씬을 추가해주세요: " + clearSceneName);
        yield break;
    }

    // 클리어 씬 미리 준비
    if (!SceneTransitionManager.PrepareScene(clearSceneName))
    {
        Debug.LogError("클리어 씬 준비 실패: " + clearSceneName);
        yield break;
    }

    // 화면 페이드아웃
    if (SubwayFadeManager.Instance != null)
    {
        yield return StartCoroutine(
            SubwayFadeManager.Instance.FadeOut(1f)
        );
    }

    // 클리어 씬 활성화
    SceneTransitionManager.ActivatePreparedScene();

    Debug.Log("클리어 씬 전환 성공: " + clearSceneName);
}

}
