using UnityEngine;

/// <summary>Prioritizes the Store R3 magic hat over the overlapping wash bucket collider.</summary>
[DisallowMultipleComponent]
public sealed class StoreR3HatPickupClick : MonoBehaviour
{
    [SerializeField] private ItemPickup itemPickup;
    [SerializeField] private Collider2D pickupCollider;

    private void Awake()
    {
        if (itemPickup == null)
            itemPickup = GetComponent<ItemPickup>();

        if (pickupCollider == null)
            pickupCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || itemPickup == null ||
            pickupCollider == null || !pickupCollider.enabled)
        {
            return;
        }

        Camera targetCamera = Camera.main;
        if (targetCamera == null)
            return;

        Ray clickRay = targetCamera.ScreenPointToRay(Input.mousePosition);
        Plane pickupPlane = new Plane(Vector3.forward, transform.position);
        if (!pickupPlane.Raycast(clickRay, out float distance))
            return;

        Vector2 clickPoint = clickRay.GetPoint(distance);
        if (pickupCollider.OverlapPoint(clickPoint))
            itemPickup.PickUpItem();
    }
}
