using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterHeadDropZone : MonoBehaviour, IDropHandler
{
    public Image headWigImage; // 주인공 머리에 나타날 이미지

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 주인공 머리에 가발 이미지 표시
        headWigImage.sprite = droppedImage.sprite;
        Color headColor = headWigImage.color;
        headColor.a = 1f;
        headWigImage.color = headColor;

        // 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color slotColor = droppedImage.color;
        slotColor.a = 0f;
        droppedImage.color = slotColor;

        Destroy(droppedItem);

        Debug.Log("가발 착용 완료!");
    }
}
