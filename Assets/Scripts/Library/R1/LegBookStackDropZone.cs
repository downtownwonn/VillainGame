using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LegBookStackDropZone : MonoBehaviour, IDropHandler
{
    [Header("진행 상태")]
    public int totalBooksNeeded = 5;
    private int stackedCount = 0;

    [Header("다리 애니메이션 / 장면 전환")]
    public Animator legAnimator;      // LegZoomPanel의 Animator
    public Image legImage;            // LegZoomPanel의 Image
    public Sprite[] stageSprites;     // 순서대로: [0]=1권, [1]=2권, [2]=3권, [3]=4권, [4]=5권

    [Header("효과음")]
    public AudioClip placeSound;
    public AudioSource audioSource;

    public void OnDrop(PointerEventData eventData)
    {
        if (stackedCount >= totalBooksNeeded) return;

        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;

        stackedCount++;

        if (audioSource != null && placeSound != null)
            audioSource.PlayOneShot(placeSound);

        // 인벤토리에서 책 소모 (사라지게 처리)
        if (draggable != null) draggable.wasDropped = true;
        droppedImage.sprite = null;
        Color c = droppedImage.color; c.a = 0f; droppedImage.color = c;
        if (draggable != null) draggable.ReturnToOriginalPosition();

        // 떨림 정지 + 장면 이미지 교체
        if (legAnimator != null) legAnimator.enabled = false;

        int index = stackedCount - 1; // 1권째 = index 0
        if (legImage != null && stageSprites != null && index >= 0 && index < stageSprites.Length && stageSprites[index] != null)
        {
            legImage.sprite = stageSprites[index];
        }

        if (stackedCount >= totalBooksNeeded)
        {
            Debug.Log("책 5권 쌓기 완료!");
        }
    }
}