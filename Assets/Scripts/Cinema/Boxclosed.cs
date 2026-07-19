using UnityEngine;

public class OpenClosedBox : MonoBehaviour
{
    public GameObject closedBox;
    public GameObject openedBox;


    void OnMouseDown()
    {
        closedBox.SetActive(false);
        openedBox.SetActive(true);
    }
}