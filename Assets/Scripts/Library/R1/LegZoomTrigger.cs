using UnityEngine;
using UnityEngine.UI;

public class LegZoomTrigger : MonoBehaviour
{
    [Header("확대 화면 패널")]
    public GameObject zoomPanel; // 다리 확대 화면 패널 (LegZoomPanel)

    [Header("확대 화면 배경")]
    [SerializeField] private Sprite zoomBackdropSprite;

    private static GameObject sharedBackdrop;
    private GameObject backdrop;

    private void Awake()
    {
        CreateBackdrop();
        SyncBackdrop();
    }

    private void LateUpdate()
    {
        SyncBackdrop();
    }

    void OnMouseDown()
    {
        Debug.Log("LegZoomTrigger: 클릭 감지됨!");

        if (zoomPanel != null)
        {
            HomeButtonBootstrap.TrackModalPanel(zoomPanel);
            Debug.Log("LegZoomTrigger: zoomPanel 활성화 시도 - " + zoomPanel.name);
            if (backdrop != null)
            {
                backdrop.SetActive(true);
                backdrop.transform.SetAsFirstSibling();
            }
            zoomPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LegZoomTrigger: zoomPanel이 연결되어 있지 않습니다!");
        }
    }

    private void CreateBackdrop()
    {
        if (zoomPanel == null || zoomBackdropSprite == null) return;

        if (sharedBackdrop != null)
        {
            backdrop = sharedBackdrop;
            return;
        }

        Canvas canvas = zoomPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        backdrop = new GameObject("Leg Zoom Fullscreen Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = backdrop.GetComponent<RectTransform>();
        rect.SetParent(canvas.rootCanvas.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.SetAsFirstSibling();

        Image image = backdrop.GetComponent<Image>();
        image.sprite = zoomBackdropSprite;
        image.color = Color.white;
        image.preserveAspect = false;
        image.raycastTarget = false;

        backdrop.SetActive(false);
        sharedBackdrop = backdrop;
    }

    private void SyncBackdrop()
    {
        bool isOpen = zoomPanel != null && zoomPanel.activeInHierarchy;
        if (backdrop == null) return;

        if (backdrop.activeSelf != isOpen)
            backdrop.SetActive(isOpen);

        if (isOpen && backdrop.transform.GetSiblingIndex() != 0)
            backdrop.transform.SetAsFirstSibling();
    }
}
