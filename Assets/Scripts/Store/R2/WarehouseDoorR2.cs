using UnityEngine;

public class WarehouseDoorR2 : MonoBehaviour
{
    public BackgroundManager2 backgroundManager2;

    private void OnMouseDown()
    {
        if (backgroundManager2 != null)
        {
            backgroundManager2.OpenWarehouse();
        }
    }
}