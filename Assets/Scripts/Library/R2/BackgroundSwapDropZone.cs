using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackgroundSwapDropZone : MonoBehaviour, IDropHandler
{
    public SpriteRenderer backgroundRenderer; // 바뀔 배경의 Sprite Renderer
    public Sprite newBackgroundSprite;        // 바뀐 후의 배경 이미지

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨!");
        
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 배경 이미지 교체
        backgroundRenderer.sprite = newBackgroundSprite;

        // 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color c = droppedImage.color;
        c.a = 0f;
        droppedImage.color = c;

        Destroy(droppedItem);

        Debug.Log("배경 교체 완료!");
    }
}