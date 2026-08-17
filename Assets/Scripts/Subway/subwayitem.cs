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

    public SangmoCharacter sangmoCharacter;

    private bool collected = false;


    private void OnMouseDown()
    {
        if (collected)
            return;

        SubwayInventoryManager.Instance.AddItem(
            inventorySprite,
            itemType
        );

        Debug.Log(itemName + " 획득");

        collected = true;

        // 상모를 얻었다면 상모 쓴 모습 → 일반 모습
        if (itemType == ItemType.Sangmo)
        {
            sangmoCharacter.ChangeToNormal();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}