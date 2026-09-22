using UnityEngine;

public class LegZoomTrigger : MonoBehaviour
{
    [Header("확대 화면 패널")]
    public GameObject zoomPanel; // 다리 확대 화면 패널 (LegZoomPanel)

    void OnMouseDown()
    {
        Debug.Log("LegZoomTrigger: 클릭 감지됨!");

        if (zoomPanel != null)
        {
            Debug.Log("LegZoomTrigger: zoomPanel 활성화 시도 - " + zoomPanel.name);
            zoomPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LegZoomTrigger: zoomPanel이 연결되어 있지 않습니다!");
        }
    }
}