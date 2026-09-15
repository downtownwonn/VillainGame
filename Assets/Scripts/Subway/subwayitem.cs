using UnityEngine;

public enum ItemType
{
    HandleLeft,
    HandleRight,
    Scissors,
    Sunglasses,
    Sangmo,
    Janggu,
    Bodyguards
}

public class SubwayItem : MonoBehaviour
{
    public string itemName;
    public Sprite inventorySprite;
    public ItemType itemType;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Item Get Popup")]
    [SerializeField] private Sprite popupItemSprite;

    public SangmoCharacter sangmoCharacter;

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

        SubwayInventoryManager.Instance.AddItem(
            itemData,
            itemType
        );

        if (ItemGetPopup.Instance != null)
        {
            ItemGetPopup.Instance.Show(popupItemSprite);
        }

        Debug.Log(itemName + " 획득");

        collected = true;

        // 상모를 얻었다면 상모 쓴 모습 → 일반 모습
        if (itemType == ItemType.Sangmo)
        {
            if (sangmoCharacter != null)
            {
                sangmoCharacter.ChangeToNormal();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}