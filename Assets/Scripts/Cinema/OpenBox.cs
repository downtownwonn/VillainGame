using UnityEngine;

public class OpenBox : MonoBehaviour
{
    public GameObject closedBox;
    public GameObject openedBox;
    public GameObject panel;

    void Start()
    {
        if (gameObject == closedBox)
        {
            panel.SetActive(false);
            openedBox.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (!openedBox.activeSelf)
        {
            closedBox.SetActive(false);
            openedBox.SetActive(true);
        }

        // 이미 열려 있어도 패널은 계속 열림
        panel.SetActive(true);
    }
}