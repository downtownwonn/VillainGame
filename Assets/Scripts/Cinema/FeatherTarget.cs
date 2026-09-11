using UnityEngine;
using UnityEngine.EventSystems;

public class FeatherTarget : MonoBehaviour, IDropHandler
{
    public VillainTickleController villain;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("FeatherTarget OnDrop 호출됨!");

        if (eventData.pointerDrag == null)
            return;

        DraggableItem item =
            eventData.pointerDrag.GetComponent<DraggableItem>();

        if (item == null)
        {
            Debug.Log("DraggableItem을 찾지 못함");
            return;
        }

        // 지금 드래그한 아이템이 진짜 깃털인지 확인
        if (item.ItemType != CinemaItemType.Feather)
        {
            Debug.Log("깃털이 아닙니다. 드롭 실패!");
            item.wasDropped = false;
            return;
        }

        // 필요하다면 신발을 먼저 얻었는지도 확인
        if (!ShoePickup.IsShoePickedUp)
        {
            Debug.Log("아직 신발을 얻지 않았습니다!");
            item.wasDropped = false;
            return;
        }

        Debug.Log("깃털 확인! 간지럼 시작!");

        item.wasDropped = true;

        if (villain != null)
        {
            villain.StartTickle(eventData.pointerDrag);
        }
    }
}