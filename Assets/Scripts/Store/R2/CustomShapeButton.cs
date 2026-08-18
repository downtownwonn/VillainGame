using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]
public class CustomShapeButton : Image
{
    private PolygonCollider2D _polygonCollider;

    protected override void Awake()
    {
        base.Awake();
        _polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // 클릭 가능 여부를 2D Collider의 영역으로 판정
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform, 
            screenPoint, 
            eventCamera, 
            out Vector3 worldPoint
        );
        return _polygonCollider.OverlapPoint(worldPoint);
    }
}