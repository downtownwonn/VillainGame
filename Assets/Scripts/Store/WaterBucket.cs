using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterBucket : MonoBehaviour, IDropHandler
{
    [Header("bini를 넣었을 때 활성화할 Hat 오브젝트")]
    [SerializeField] private GameObject hatObject; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Image droppedImage = droppedObject.GetComponent<Image>();

            // 스프라이트 이름에 'bini'가 포함되어 있는지 확인 (대소문자 구분 없음)
            if (droppedImage != null && droppedImage.sprite != null && droppedImage.sprite.name.ToLower().Contains("bini"))
            {
                Debug.Log($"bini 아이템({droppedImage.sprite.name})을 물통에 넣었습니다!");

                DraggableItemofStore draggable = droppedObject.GetComponent<DraggableItemofStore>();
                if (draggable != null)
                {
                    draggable.wasDropped = true;
                    draggable.ClearItem(); // bini 이미지 지우고 슬롯 원래 위치로 복귀
                }

                // Hat 오브젝트 켜주기
                if (hatObject != null)
                {
                    hatObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("hatObject가 인스펙터에 할당되지 않았습니다!");
                }
            }
        }
    }
}