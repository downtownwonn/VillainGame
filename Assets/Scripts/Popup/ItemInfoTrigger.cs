using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoTrigger : MonoBehaviour, IPointerClickHandler
{
    private Image itemImage;
    private ItemInfoData itemInfoData;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        itemInfoData = GetComponent<ItemInfoData>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemImage == null || itemImage.sprite == null)
        {
            return;
        }

        if (itemInfoData == null)
        {
            Debug.LogWarning(
                "ItemIcon에 ItemInfoData가 없습니다."
            );
            return;
        }

        if (itemInfoData.itemData == null)
        {
            Debug.LogWarning(
                "현재 슬롯에 ItemData가 없습니다."
            );
            return;
        }

        if (ItemInfoPopup.Instance == null)
        {
            Debug.LogWarning(
                "ItemInfoPopup이 존재하지 않습니다."
            );
            return;
        }

        ItemData data = itemInfoData.itemData;

        ItemInfoPopup.Instance.Show(
            data.itemSprite,
            data.itemName,
            data.description
        );
    }
}