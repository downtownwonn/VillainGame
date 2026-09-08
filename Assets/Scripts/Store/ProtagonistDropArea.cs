using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class ProtagonistDropArea : MonoBehaviour, IDropHandler
{
    [Header("부위별 착용 슬롯 Images")]
    public Image headSlot; // 머리
    public Image handSlot; // 손
    public Image glasses_bodySlot; // 몸통,안경

    [Header("씬 전환 설정")]
    public Image fadeOverlay; // 화면 전체를 덮는 검은색 UI Image
    public string nextSceneName; // 이동할 씬 이름
    public float delayBeforeFade = 1.0f; // 슬롯 완료 후 대기 시간 (1초)
    public float fadeDuration = 1.0f; // 페이드 아웃에 걸리는 시간 (1초)

    private bool isTransitioning = false;

    public void OnDrop(PointerEventData eventData)
    {
        // 씬 전환 진행 중일 때는 드롭 처리 중단
        if (isTransitioning) return;

        DraggableItemofStore item = eventData.pointerDrag?.GetComponent<DraggableItemofStore>();

        if (item == null)
            return;

        Image itemImage = item.GetComponent<Image>();

        if (itemImage != null && itemImage.sprite != null)
        {
            string spriteName = itemImage.sprite.name;
            bool isEquippedSuccess = false;

            // 1. 머리 장착 조건
            if (spriteName.Contains("mophead") || spriteName.Contains("hat") || spriteName.Contains("Squ"))
            {
                EquipItem(headSlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("머리에 장착 완료!");
            }
            // 2. 손 장착 조건
            else if (spriteName.Contains("glove") || spriteName.Contains("bong"))
            {
                EquipItem(handSlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("손에 장착 완료!");
            }
            // 3. 몸통 장착 조건
            else if (spriteName.Contains("apron") || spriteName.Contains("glasses"))
            {
                EquipItem(glasses_bodySlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("몸통or안경에 장착 완료!");
            }
            else
            {
                Debug.LogWarning("맞는 장착 부위가 없습니다: " + spriteName);
            }

            // ★ 맞는 부위에 장착되었을 때만 비우기 처리
            if (isEquippedSuccess)
            {
                item.wasDropped = true;
                item.ClearItem();

                // 모든 슬롯이 채워졌는지 검사
                CheckAllSlotsEquipped();
            }
        }
    }

    private void EquipItem(Image targetSlot, Sprite newSprite)
    {
        if (targetSlot != null)
        {
            targetSlot.sprite = newSprite;

            Color c = targetSlot.color;
            c.a = 1f; // 알파값 100%로 켜주기
            targetSlot.color = c;
            targetSlot.gameObject.SetActive(true);
        }
    }

    // 3개 슬롯이 모두 채워졌는지 확인하는 메서드
    private void CheckAllSlotsEquipped()
    {
        if (IsSlotFilled(headSlot) && IsSlotFilled(handSlot) && IsSlotFilled(glasses_bodySlot))
        {
            isTransitioning = true;
            StartCoroutine(FadeAndChangeSceneRoutine());
        }
    }

    // 슬롯이 활성화되어 있고 스프라이트가 존재하는지 체크
    private bool IsSlotFilled(Image slot)
    {
        return slot != null && slot.gameObject.activeSelf && slot.sprite != null;
    }

    // 1초 대기 -> Fade Out 코루틴
    private IEnumerator FadeAndChangeSceneRoutine()
    {
        // 1. 설정한 대기시간(1초)만큼 대기
        yield return new WaitForSeconds(delayBeforeFade);

        // 2. Fade Overlay 활성화
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);

            float timer = 0f;
            Color fadeColor = fadeOverlay.color;

            // 점점 어두워지기 (Fade Out)
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeColor.a = Mathf.Clamp01(timer / fadeDuration);
                fadeOverlay.color = fadeColor;
                yield return null;
            }
        }

        // 3. 씬 전환
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next Scene Name이 지정되지 않았습니다!");
        }
    }
}