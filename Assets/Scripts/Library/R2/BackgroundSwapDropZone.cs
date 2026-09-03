using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackgroundSwapDropZone : MonoBehaviour, IDropHandler
{
    public SpriteRenderer backgroundRenderer; // 남자머리의 Sprite Renderer
    public Sprite newBackgroundSprite;        // 금발 장발 이미지

    [Header("가발로 바뀔 때 적용할 위치/크기")]
    public Vector3 wigLocalPosition = new Vector3(-2.1f, 0.67f, 0f);
    public Vector3 wigLocalScale = new Vector3(0.25f, 0.25f, 1f);

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨!");

        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 이미지 교체
        backgroundRenderer.sprite = newBackgroundSprite;

        // 가발 전용 위치/크기 적용
        backgroundRenderer.transform.localPosition = wigLocalPosition;
        backgroundRenderer.transform.localScale = wigLocalScale;

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
    }
}