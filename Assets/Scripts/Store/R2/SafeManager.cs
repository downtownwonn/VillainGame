using UnityEngine;

public class SafeManager : MonoBehaviour
{
    // 유니티 에디터에서 드래그해서 연결할 팝업 창 (PopupUI)
    public GameObject popupUI;

    // 금고(이미지 버튼)를 클릭했을 때 실행할 함수
    public void OpenPopup()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(true); // 꺼져있던 PopupUI 켜기
        }
    }

    // 팝업 내부의 [X] 닫기 버튼을 클릭했을 때 실행할 함수
    public void ClosePopup()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(false); // PopupUI 다시 숨기기
        }
    }
}