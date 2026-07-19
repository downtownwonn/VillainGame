using UnityEngine;

public class OpenBoxUI : MonoBehaviour
{
    public GameObject panel;


    void OnMouseDown()
    {
        panel.SetActive(true);
    }
}