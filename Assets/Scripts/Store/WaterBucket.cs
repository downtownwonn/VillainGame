using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterBucket : MonoBehaviour, IDropHandler
{
    [Header("bini를 넣었을 때 활성화할 Hat 오브젝트")]
    [SerializeField] private GameObject hatObject; 

    [Header("mophandle을 넣었을 때 활성화할 Black Mop Handle 오브젝트")]
    [SerializeField] private GameObject blackMopHandleObject;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Image droppedImage = droppedObject.GetComponent<Image>();

            if (droppedImage != null && droppedImage.sprite != null)
            {
                string spriteName = droppedImage.sprite.name.ToLower();

                // 1. bini 처리
                if (spriteName.Contains("bini"))
                {
                    Debug.Log($"bini 아이템({droppedImage.sprite.name})을 물통에 넣었습니다!");

                    ClearDroppedItem(droppedObject);

                    if (hatObject != null)
                    {
                        hatObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("hatObject가 인스펙터에 할당되지 않았습니다!");
                    }
                }
                // 2. mophandle 처리
                else if (spriteName.Contains("mophandle"))
                {
                    Debug.Log($"mophandle 아이템({droppedImage.sprite.name})을 물통에 넣었습니다!");

                    ClearDroppedItem(droppedObject);

                    if (blackMopHandleObject != null)
                    {
                        blackMopHandleObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("blackMopHandleObject가 인스펙터에 할당되지 않았습니다!");
                    }
                }
            }
        }
    }

    // 아이템 초기화 중복 코드 분리
    private void ClearDroppedItem(GameObject droppedObject)
    {
        DraggableItemofStore draggable = droppedObject.GetComponent<DraggableItemofStore>();
        if (draggable != null)
        {
            draggable.wasDropped = true;
            draggable.ClearItem(); // 이미지 지우고 슬롯 원래 위치로 복귀
        }
    }
}