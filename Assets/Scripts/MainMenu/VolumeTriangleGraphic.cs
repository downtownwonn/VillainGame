using UnityEngine;
using UnityEngine.UI;

/// <summary>A slim tapered volume gauge, with the wide end at maximum.</summary>
public sealed class VolumeTriangleGraphic : MaskableGraphic
{
    public float maximumWidth = 8f;

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();
        Rect rect = GetPixelAdjustedRect();
        float halfWidth = Mathf.Min(maximumWidth, rect.width) * 0.5f;
        vertexHelper.AddVert(new Vector3(rect.center.x, rect.yMin), color, Vector2.zero);
        vertexHelper.AddVert(new Vector3(rect.center.x - halfWidth, rect.yMax), color, Vector2.up);
        vertexHelper.AddVert(new Vector3(rect.center.x + halfWidth, rect.yMax), color, Vector2.one);
        vertexHelper.AddTriangle(0, 1, 2);
    }
}
