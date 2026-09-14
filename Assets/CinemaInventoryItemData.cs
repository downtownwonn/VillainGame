using UnityEngine;

public enum CinemaItemType
{
    None,
    Shoe,
    Feather
}

public class CinemaInventoryItemData : MonoBehaviour
{
    public CinemaItemType itemType = CinemaItemType.None;
}