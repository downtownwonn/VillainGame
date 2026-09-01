using UnityEngine;
using UnityEngine.EventSystems;

public class FeatherTarget : MonoBehaviour, IDropHandler
{
    public VillainTickleController villain;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(" FeatherTarget OnDrop 호출됨!");

        if (eventData.pointerDrag == null)
            return;

        DraggableItem item =
            eventData.pointerDrag.GetComponent<DraggableItem>();

        if (item == null)
        {
            Debug.Log(" DraggableItem을 찾지 못함");
            return;
        }

        // =====================================
        // 신발을 먼저 얻었는지 확인
        // =====================================

        if (!ShoePickup.IsShoePickedUp)
        {
            Debug.Log(" 아직 신발을 얻지 않았습니다!");

            // 드롭 실패
            item.wasDropped = false;

            return;
        }

        // =====================================
        // 신발을 얻었다면 깃털 사용 가능
        // =====================================

        Debug.Log(" 신발을 얻었으므로 깃털 사용 가능!");

        item.wasDropped = true;

        if (villain != null)
        {
            villain.StartTickle(eventData.pointerDrag);
        }
    }
}