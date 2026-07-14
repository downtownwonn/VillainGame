using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VillainDropZone : MonoBehaviour, IDropHandler
{
    public Transform stackParent;      // 책이 쌓일 위치 (의자 밑)
    public GameObject stackedBookPrefab; // 쌓인 책 하나를 표현할 이미지 프리팹
    public float stackHeight = 15f;    // 책 한 권마다 올라갈 높이
    public int totalBooksNeeded = 5;   // 몇 권 모으면 퇴치되는지

    private int stackedCount = 0;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop 호출됨! pointerDrag = " + eventData.pointerDrag);

        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggable = droppedItem.GetComponent<DraggableItem>();
        if (draggable != null) draggable.wasDropped = true;

        Image droppedImage = droppedItem.GetComponent<Image>();
        if (droppedImage == null || droppedImage.sprite == null) return;


        // 쌓인 책 이미지 생성
        GameObject newBook = Instantiate(stackedBookPrefab, stackParent);
        newBook.GetComponent<Image>().sprite = droppedImage.sprite;

        RectTransform rt = newBook.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, stackedCount * stackHeight);

        stackedCount++;

        // 원래 인벤토리 슬롯 비우기
        droppedImage.sprite = null;
        Color c = droppedImage.color;
        c.a = 0f;
        droppedImage.color = c;

        // 드래그했던 아이템 원래 자리로 (부모가 Canvas로 옮겨져 있으니 파괴하거나 초기화)
        Destroy(droppedItem);

        if (stackedCount >= totalBooksNeeded)
        {
            Debug.Log("빌런 퇴치 성공!");
            // 여기에 퇴치 애니메이션/로직 연결
        }
    }
}