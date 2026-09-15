using UnityEngine;

public class SunglassesItem : MonoBehaviour
{
    public Sprite inventorySprite;
    public SunglassesCharacter character;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Item Get Popup")]
    [SerializeField] private Sprite popupItemSprite;

    private bool collected = false;

    private void OnMouseDown()
    {
        if (collected)
            return;

        if (SubwayInventoryManager.Instance == null)
        {
            Debug.LogError("SubwayInventoryManager를 찾을 수 없습니다.");
            return;
        }

        if (itemData == null)
        {
            Debug.LogError(gameObject.name + "에 ItemData가 연결되지 않았습니다.");
            return;
        }

        // 선글라스를 인벤토리에 넣기
        SubwayInventoryManager.Instance.AddItem(
            itemData,
            ItemType.Sunglasses
        );

        // 아이템 획득 팝업
        if (ItemGetPopup.Instance != null)
        {
            ItemGetPopup.Instance.Show(popupItemSprite);
        }

        Debug.Log("선글라스 획득!");

        collected = true;

        // 어르신을 선글라스 벗은 모습으로 변경
        if (character != null)
        {
            character.RemoveSunglasses();
        }
        else
        {
            Debug.LogError("SunglassesCharacter가 연결되지 않았습니다.");
        }
    }
}