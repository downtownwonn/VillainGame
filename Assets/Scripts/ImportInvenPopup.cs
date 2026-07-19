using UnityEngine;

public class InventoryPopup : MonoBehaviour
{
    public GameObject itemPopup;

    public void OpenPopup()
    {
        itemPopup.SetActive(true);
    }
}