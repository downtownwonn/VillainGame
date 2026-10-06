using UnityEngine;
using UnityEngine.UI;

public class ArrowLockOnPanel : MonoBehaviour
{
    [Header("이 중 하나라도 열려 있으면 화살표 잠금")]
    public GameObject[] panels;

    [Header("잠글 화살표 버튼들")]
    public GameObject[] arrowButtons;

    [Header("체크하면 잠글 때 화살표를 아예 숨김")]
    public bool hideArrows = false;

    private bool lastLocked = false;

    void Start()
    {
        Apply(IsAnyPanelOpen());
    }

    void Update()
    {
        bool locked = IsAnyPanelOpen();
        if (locked != lastLocked)
        {
            Apply(locked);
        }
    }

    bool IsAnyPanelOpen()
    {
        foreach (GameObject p in panels)
        {
            if (p != null && p.activeInHierarchy) return true;
        }
        return false;
    }

    void Apply(bool locked)
    {
        lastLocked = locked;

        foreach (GameObject arrow in arrowButtons)
        {
            if (arrow == null) continue;

            if (hideArrows)
            {
                arrow.SetActive(!locked);
                continue;
            }

            // Button이면 클릭 비활성화
            Button btn = arrow.GetComponent<Button>();
            if (btn != null) btn.interactable = !locked;

            // Button이 아닌 커스텀 클릭이어도 막히도록 레이캐스트도 끔
            Graphic g = arrow.GetComponent<Graphic>();
            if (g != null) g.raycastTarget = !locked;
        }
    }
}