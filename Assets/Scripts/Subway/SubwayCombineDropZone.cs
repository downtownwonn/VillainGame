using UnityEngine;
using UnityEngine.EventSystems;

public class SubwayCombineDropZone : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
    Debug.Log("ENTER");
    Debug.Log("pointerDrag = " + eventData.pointerDrag);
    Debug.Log("pointerCurrentRaycast = " + eventData.pointerCurrentRaycast.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXIT");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROP");
    }
}