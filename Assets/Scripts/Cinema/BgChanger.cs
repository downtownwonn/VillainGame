using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    [Header("Backgrounds")]
    public GameObject mainBackground;
    public GameObject leftBackground;
    public GameObject rightBackground;

    [Header("Heart Effect")]
    [Tooltip("하트 효과가 있는 경우에만 연결하세요. 없는 씬에서는 비워두면 됩니다.")]
    public KissHeartEffect heartEffect;

    [Header("Navigation Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Optional navigation blocker")]
    [Tooltip("이 패널이 열려 있는 동안 배경 전환 화살표를 무시합니다.")]
    public GameObject navigationBlockerPanel;

    [Header("Optional R3 speaker icon")]
    public VillainHeadDropZone villainHeadDropZone;

    // 0 = Left
    // 1 = Main
    // 2 = Right
    private int currentView = 1;
    private bool navigationBlockStateInitialized;
    private bool wasNavigationBlocked;
    private bool leftButtonWasEnabled;
    private bool rightButtonWasEnabled;


    void Start()
    {
        leftButtonWasEnabled = leftButton == null || leftButton.enabled;
        rightButtonWasEnabled = rightButton == null || rightButton.enabled;
        UpdateView();
    }

    private void Update()
    {
        bool blocked = IsNavigationBlocked();
        if (navigationBlockStateInitialized && blocked == wasNavigationBlocked)
            return;

        navigationBlockStateInitialized = true;
        wasNavigationBlocked = blocked;

        if (leftButton != null)
            leftButton.enabled = blocked ? false : leftButtonWasEnabled;
        if (rightButton != null)
            rightButton.enabled = blocked ? false : rightButtonWasEnabled;
    }


    public void Left()
    {
        if (IsNavigationBlocked()) return;

        if (currentView > 0)
        {
            CloseSubwayItemPopup();

            currentView--;

            UpdateView();
        }
    }


    public void Right()
    {
        if (IsNavigationBlocked()) return;

        if (currentView < 2)
        {
            CloseSubwayItemPopup();

            currentView++;

            UpdateView();
        }
    }

    private bool IsNavigationBlocked()
    {
        return navigationBlockerPanel != null && navigationBlockerPanel.activeInHierarchy;
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


    private void UpdateView()
    {
        // ==========================================
        // 배경 전환
        // ==========================================

        if (mainBackground != null)
        {
            mainBackground.SetActive(currentView == 1);
        }

        if (leftBackground != null)
        {
            leftBackground.SetActive(currentView == 0);
        }

        if (rightBackground != null)
        {
            rightBackground.SetActive(currentView == 2);
        }

        if (villainHeadDropZone != null)
        {
            villainHeadDropZone.SetMainViewVisible(currentView == 1);
        }


        // ==========================================
        // 이동 버튼
        // ==========================================

        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(currentView != 0);
        }

        if (rightButton != null)
        {
            rightButton.gameObject.SetActive(currentView != 2);
        }


        // ==========================================
        // 하트 표시 여부
        //
        // 중요:
        // heartEffect 자체를 SetActive(false) 하지 않는다.
        // 하트 생성은 계속 진행하고,
        // 화면에 보이는 것만 조절한다.
        // ==========================================

        if (heartEffect != null)
        {
            heartEffect.SetHeartVisible(currentView == 2);
        }
    }
}
