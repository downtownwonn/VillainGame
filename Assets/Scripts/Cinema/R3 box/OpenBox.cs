using UnityEngine;

public class OpenBox : MonoBehaviour
{
    public GameObject closedBox;
    public GameObject openedBox;
    public GameObject panel;

    private bool canOpen = false;

    void Start()
    {
        canOpen = false;

        if (gameObject == closedBox)
        {
            panel.SetActive(false);
            openedBox.SetActive(false);
        }
    }

    // 사람이 숙인 후 ChangePlayer에서 호출
    public void EnableBoxClick()
    {
        canOpen = true;

        Debug.Log("박스 클릭 가능!");
    }

    void OnMouseDown()
    {
        // 사람이 숙이기 전에는 아무것도 하지 않음
        if (!canOpen)
        {
            Debug.Log("아직 박스를 열 수 없습니다.");
            return;
        }

        // 박스 열기
        if (!openedBox.activeSelf)
        {
            closedBox.SetActive(false);
            openedBox.SetActive(true);
        }

        // 퍼즐 패널 열기
        panel.SetActive(true);
    }
}