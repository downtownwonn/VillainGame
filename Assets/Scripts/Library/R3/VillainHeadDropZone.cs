using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VillainHeadDropZone : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Image headSpeakerImage;   // 빌런 머리 위에 나타날 이미지
    public Sprite muteSprite;        // 클릭하면 바뀔 무음 스피커 이미지

    public SpriteRenderer backgroundRenderer; // 바뀔 배경의 Sprite Renderer
    public Sprite newBackgroundSprite;        // 바뀐 후의 배경 이미지

    private bool isPlaced = false;   // 스피커가 놓였는지 여부

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        // 빌런 머리 위에 스피커 이미지 표시
        headSpeakerImage.sprite = droppedImage.sprite;
        Color c = headSpeakerImage.color;
        c.a = 1f;
        headSpeakerImage.color = c;

        isPlaced = true;

        // 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color slotColor = droppedImage.color;
        slotColor.a = 0f;
        droppedImage.color = slotColor;

        Destroy(droppedItem);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 스피커가 놓인 상태에서 클릭하면 무음으로 교체
        if (isPlaced && muteSprite != null)
        {
            headSpeakerImage.sprite = muteSprite;
            Debug.Log("무음으로 변경됨!");

            // 배경도 함께 교체
            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }
        }
    }
}