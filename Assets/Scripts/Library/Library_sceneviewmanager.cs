using UnityEngine;

public class SceneViewManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject leftShelfView;
    public GameObject rightShelfView;

    public GameObject mainViewUI;
    public GameObject leftShelfUI;
    public GameObject rightShelfUI;

    public void GoToLeftShelf()
    {
        mainView.SetActive(false);
        leftShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        leftShelfUI.SetActive(true);
    }

    public void GoToRightShelf()
    {
        mainView.SetActive(false);
        rightShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        rightShelfUI.SetActive(true);
    }

    public void GoToMainView()
    {
        leftShelfView.SetActive(false);
        rightShelfView.SetActive(false);

        leftShelfUI.SetActive(false);
        rightShelfUI.SetActive(false);

        mainView.SetActive(true);   // ← 이 줄 추가!
        mainViewUI.SetActive(true);
    }
}