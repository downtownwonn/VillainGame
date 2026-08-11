using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WarehouseLockDrop : MonoBehaviour, IDropHandler
{
    public BackgroundManager backgroundManager;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItemofStore item = eventData.pointerDrag.GetComponent<DraggableItemofStore>();

        if (item == null)
            return;

        item.wasDropped = true;

        // 인벤토리에서 열쇠 제거
        Image itemImage = item.GetComponent<Image>();
        itemImage.sprite = null;
        itemImage.color = new Color(1, 1, 1, 0);

        // 창고 잠금 해제
        backgroundManager.UnlockWarehouse();

        // 자물쇠 서서히 사라짐
        StartCoroutine(FadeOutLock());
    }

    private IEnumerator FadeOutLock()
    {
        Image lockImage = GetComponent<Image>();

        Color color = lockImage.color;

        float fadeTime = 1f;
        float currentTime = 0f;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, currentTime / fadeTime);

            lockImage.color = color;

            yield return null;
        }

        // 완전히 투명하게
        color.a = 0f;
        lockImage.color = color;

        // 클릭을 막지 않도록 설정
        lockImage.raycastTarget = false;

        // 버튼도 비활성화
        Button button = GetComponent<Button>();
        if (button != null)
            button.enabled = false;

        // 드롭 스크립트도 비활성화
        enabled = false;

        // 마지막에 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}