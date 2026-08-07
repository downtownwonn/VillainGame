using UnityEngine;

public class SceneViewManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject leftShelfView;
    public GameObject rightShelfView;

    public GameObject mainViewUI;
    public GameObject leftShelfUI;
    public GameObject rightShelfUI;

    public GameObject stackedBooksContainer;
    public GameObject villainDropZone; // ★ 새로 추가

    public void GoToLeftShelf()
    {
        Debug.Log("=== GoToLeftShelf 실행됨 ===");

        mainView.SetActive(false);
        leftShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        leftShelfUI.SetActive(true);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(false);

        if (villainDropZone != null)
            villainDropZone.SetActive(false); // ★ 드롭존도 끄기
    }

    public void GoToRightShelf()
    {
        mainView.SetActive(false);
        rightShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        rightShelfUI.SetActive(true);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(false);

        if (villainDropZone != null)
            villainDropZone.SetActive(false); // ★ 드롭존도 끄기
    }

    public void GoToMainView()
    {
        leftShelfView.SetActive(false);
        rightShelfView.SetActive(false);

        leftShelfUI.SetActive(false);
        rightShelfUI.SetActive(false);

        mainView.SetActive(true);
        mainViewUI.SetActive(true);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(true);

        if (villainDropZone != null)
            villainDropZone.SetActive(true); // ★ 다시 켜기
    }
}