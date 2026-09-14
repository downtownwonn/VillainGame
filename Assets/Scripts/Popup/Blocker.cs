using UnityEngine;
using UnityEngine.EventSystems;

public class PopupBlocker : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("�ڡڡڡڡ� BLOCKER CLICK �ڡڡڡڡ�");
    }
}