using UnityEngine;

public class LaptopScreenTrigger : MonoBehaviour
{
    public GameObject laptopScreenPanel; // 인스펙터에서 연결할 노트북 화면 패널

    void OnMouseDown()
    {
        Debug.Log("노트북 클릭됨");

        if (laptopScreenPanel != null)
        {
            laptopScreenPanel.SetActive(true);
        }
    }
}