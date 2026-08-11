using UnityEngine;
using UnityEngine.UI;

public class library_BgChanger : MonoBehaviour 
{
    public GameObject mainBackground;
    public GameObject leftBackground;
    public GameObject rightBackground;

    public Button leftButton;
    public Button rightButton;

    public GameObject villainDropZone;
    public GameObject stackedBooksContainer;
    private int currentView = 1;

    void Start()
    {
        UpdateView();
    }

    public void Left()
    {
        if (currentView > 0)
        {
            currentView--;
            UpdateView();
        }
    }

    public void Right()
    {
        if (currentView < 2)
        {
            currentView++;
            UpdateView();
        }
    }

    void UpdateView()
    {
        mainBackground.SetActive(currentView == 1);
        leftBackground.SetActive(currentView == 0);
        rightBackground.SetActive(currentView == 2);

        leftButton.gameObject.SetActive(currentView != 0);
        rightButton.gameObject.SetActive(currentView != 2);

        if (villainDropZone != null)
            villainDropZone.SetActive(currentView == 1);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(currentView == 1);
    }
}