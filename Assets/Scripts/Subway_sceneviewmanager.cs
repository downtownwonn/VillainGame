using UnityEngine;

public class Subway_sceneviewmanager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject leftShelfView;
    public GameObject rightShelfView;

    public GameObject mainViewUI;
    public GameObject leftShelfUI;
    public GameObject rightShelfUI;

    public void GoToLeftShelf()
    {
        if (SubwayItemPopupManager.Instance != null)
        {
            SubwayItemPopupManager.Instance.HidePopup();
        }

        mainView.SetActive(false);
        leftShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        leftShelfUI.SetActive(true);
    }

    public void GoToRightShelf()
    {
        if (SubwayItemPopupManager.Instance != null)
        {
            SubwayItemPopupManager.Instance.HidePopup();
        }

        mainView.SetActive(false);
        rightShelfView.SetActive(true);

        mainViewUI.SetActive(false);
        rightShelfUI.SetActive(true);
    }

    public void GoToMainView()
    {
        if (SubwayItemPopupManager.Instance != null)
        {
            SubwayItemPopupManager.Instance.HidePopup();
        }

        leftShelfView.SetActive(false);
        rightShelfView.SetActive(false);

        leftShelfUI.SetActive(false);
        rightShelfUI.SetActive(false);

        mainView.SetActive(true);
        mainViewUI.SetActive(true);
    }
}