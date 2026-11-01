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

        // 맨 왼쪽(0)이면 왼쪽 화살표 숨김, 맨 끝(MaxView)이면 오른쪽 화살표 숨김
        // R1 (hasRightView 꺼짐): 메인 = 왼쪽 화살표만, 왼쪽 화면 = 오른쪽 화살표만
        // R2, R3 (hasRightView 켜짐): 기존 동작과 동일
        leftButton.gameObject.SetActive(currentView != 0);
        rightButton.gameObject.SetActive(currentView != MaxView);

        if (villainDropZone != null)
            villainDropZone.SetActive(currentView == 1);

        if (stackedBooksContainer != null)
            stackedBooksContainer.SetActive(currentView == 1);
    }
}