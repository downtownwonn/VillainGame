using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WarehouseLockDrop : MonoBehaviour, IDropHandler
{
    public BackgroundManager backgroundManager;

    [Header("사운드 설정")]
    [Tooltip("자물쇠가 풀릴 때 재생할 효과음 클립")]
    public AudioClip unlockSound; 

    private bool isUnlocked = false; // 자물쇠가 이미 해제되었는지 여부

    public void OnDrop(PointerEventData eventData)
    {
        if (isUnlocked) return; // 이미 해제 중이거나 완료된 경우 중복 실행 방지

        DraggableItemofStore item = eventData.pointerDrag.GetComponent<DraggableItemofStore>();

        if (item == null)
            return;

        item.wasDropped = true;
        isUnlocked = true;

        // 🔊 1. 자물쇠 풀리는 소리 재생
        if (unlockSound != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, Camera.main.transform.position);
        }

        // 인벤토리에서 열쇠 제거
        Image itemImage = item.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.color = new Color(1, 1, 1, 0);
        }

        // 창고 잠금 해제
        if (backgroundManager != null)
        {
            backgroundManager.UnlockWarehouse();
        }

        // 자물쇠 서서히 사라짐
        StartCoroutine(FadeOutLock());
    }

    private IEnumerator FadeOutLock()
    {
        Image lockImage = GetComponent<Image>();

        if (lockImage != null)
        {
            Color color = lockImage.color;

            float fadeTime = 3f;
            float currentTime = 0f;

            while (currentTime < fadeTime)
            {
                currentTime += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
                lockImage.color = color;
                yield return null;
            }
        }

        // 완전히 해제 처리 진행
        CompleteUnlock();
    }

    // 오브젝트가 비활성화될 때(배경 이동 등) 코루틴이 멈추는 것에 대한 대비
    private void OnDisable()
    {
        // 페이드아웃 중에 화면이 전환되어 비활성화되면 즉시 정리 완료 처리
        if (isUnlocked)
        {
            CompleteUnlock();
        }
    }

    // 자물쇠 비활성화 및 상태 최종 정리 함수
    private void CompleteUnlock()
    {
        Image lockImage = GetComponent<Image>();
        if (lockImage != null)
        {
            Color color = lockImage.color;
            color.a = 0f;
            lockImage.color = color;
            lockImage.raycastTarget = false;
        }

        Button button = GetComponent<Button>();
        if (button != null)
            button.enabled = false;

        enabled = false;
        gameObject.SetActive(false);
    }
}