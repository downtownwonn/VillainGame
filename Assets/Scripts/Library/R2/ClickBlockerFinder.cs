using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBlockerFinder : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || EventSystem.current == null) return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count == 0)
        {
            Debug.Log("[클릭검사] 마우스 아래 UI 없음 → 말풍선 떠야 정상");
            return;
        }

        foreach (RaycastResult r in results)
        {
            Debug.Log("[클릭검사] 클릭을 막는 오브젝트: " + GetPath(r.gameObject.transform));
        }
    }

    string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}