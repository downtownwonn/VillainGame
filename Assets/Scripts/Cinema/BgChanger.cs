using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    public GameObject mainBackground;
    public GameObject leftBackground;
    public GameObject rightBackground;

    public Button leftButton;
    public Button rightButton;

    private int currentView = 1;

    void Start()
    {
        UpdateView();
    }

    public void Left()
    {
        if (currentView > 0)
        {
            CloseSubwayItemPopup();

            currentView--;
            UpdateView();
        }
    }

    public void Right()
    {
        if (currentView < 2)
        {
            CloseSubwayItemPopup();

            currentView++;
            UpdateView();
        }
    }

    private void CloseSubwayItemPopup()
    {
         // 지하철 아이템 팝업
        if (SubwayItemPopupManager.Instance != null)
           {
            SubwayItemPopupManager.Instance.HidePopup();
           }

        // 기존 말풍선
        if (BubbleManager.Instance != null)
           {
            BubbleManager.Instance.HideBubble();
           }

        // 지하철 전용 말풍선
        if (SubwayBubbleManager.Instance != null)
           {
            SubwayBubbleManager.Instance.HideBubble();
           }
    }

    void UpdateView()
    {
        mainBackground.SetActive(currentView == 1);
        leftBackground.SetActive(currentView == 0);
        rightBackground.SetActive(currentView == 2);

        leftButton.gameObject.SetActive(currentView != 0);
        rightButton.gameObject.SetActive(currentView != 2);
    }
}