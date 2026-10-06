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

    [Header("오른쪽 화면 사용 여부 (R1만 끄기)")]
    public bool hasRightView = true;

    // 0 = 왼쪽, 1 = 메인, 2 = 오른쪽
    private int currentView = 1;

    int MaxView
    {
        get { return hasRightView ? 2 : 1; }
    }

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
        if (currentView < MaxView)
        {
            currentView++;
            UpdateView();
        }
    }

    void UpdateView()
    {
        mainBackground.SetActive(currentView == 1);
        leftBackground.SetActive(currentView == 0);

        if (rightBackground != null)
            rightBackground.SetActive(hasRightView && currentView == 2);

        if (hasRightView)
        {
            // 기존 동작 그대로 (R2, R3 등)
            leftButton.gameObject.SetActive(currentView != 0);
            rightButton.gameObject.SetActive(currentView != 2);
        }
        else
        {
            // R1: 메인 = 왼쪽 화살표만, 왼쪽 화면 = 좌우 화살표 모두
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(currentView == 0);
        }

        if (villainDropZone != null)
            villainDropZone.SetActive(currentView == 1);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(currentView == 1);
    }
}