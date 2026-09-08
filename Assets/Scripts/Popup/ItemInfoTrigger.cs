using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfoTrigger : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Information")]
    [SerializeField] private string itemName;

    [TextArea(3, 6)]
    [SerializeField] private string itemDescription;

    private UnityEngine.UI.Image itemImage;

    private void Awake()
    {
        itemImage = GetComponent<UnityEngine.UI.Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemImage == null)
        {
            return;
        }

        if (itemImage.sprite == null)
        {
            return;
        }

        if (ItemInfoPopup.Instance == null)
        {
            Debug.LogWarning("ItemInfoPopup이 존재하지 않습니다.");
            return;
        }

        ItemInfoPopup.Instance.Show(
            itemImage.sprite,
            itemName,
            itemDescription
        );
    }
}