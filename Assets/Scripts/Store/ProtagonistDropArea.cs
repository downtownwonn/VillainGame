using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProtagonistDropArea : MonoBehaviour, IDropHandler
{
    [Header("부위별 착용 슬롯 Images")]
    public Image headSlot; // 머리
    public Image handSlot; // 손
    public Image bodySlot; // 몸통
    public Image glassesSlot; // 안경

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItemofStore item = eventData.pointerDrag?.GetComponent<DraggableItemofStore>();

        if (item == null)
            return;

        Image itemImage = item.GetComponent<Image>();

        if (itemImage != null && itemImage.sprite != null)
        {
            string spriteName = itemImage.sprite.name;
            bool isEquippedSuccess = false;

            // 1. 머리 장착 조건
            if (spriteName.Contains("mophead") || spriteName.Contains("hat") || spriteName.Contains("Squ"))
            {
                EquipItem(headSlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("머리에 장착 완료!");
                
            }
            // 2. 손 장착 조건
            else if (spriteName.Contains("rubbergloves") || spriteName.Contains("blackmophandle"))
            {
                EquipItem(handSlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("손에 장착 완료!");
                
            }
            // 3. 몸통 장착 조건
            else if (spriteName.Contains("apron"))
            {
                EquipItem(bodySlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("몸통에 장착 완료!");
            }

            // 4. 안경 장착 조건
            else if (spriteName.Contains("glasses"))
            {
                EquipItem(glassesSlot, itemImage.sprite);
                isEquippedSuccess = true;
                Debug.Log("손에 장착 완료!");
            }
            else
            {
                Debug.LogWarning("맞는 장착 부위가 없습니다: " + spriteName);
            }

            // ★ 맞는 부위에 장착되었을 때만 열쇠 코드(WarehouseLockDrop)와 똑같은 방식으로 비우기!
            if (isEquippedSuccess)
            {
                item.wasDropped = true;

                // 기존 ClearItem()을 호출해서 투명하게 비워줍니다.
                item.ClearItem(); 
            }
        }
    }

    private void EquipItem(Image targetSlot, Sprite newSprite)
    {
        if (targetSlot != null)
        {
            targetSlot.sprite = newSprite;

            Color c = targetSlot.color;
            c.a = 1f; // 알파값 100%로 켜주기
            targetSlot.color = c;
            targetSlot.gameObject.SetActive(true);
        }
    }
}