using UnityEngine;
using UnityEngine.UI;

public class LegZoomTrigger : MonoBehaviour
{
    [Header("확대 화면 패널")]
    public GameObject zoomPanel; // 다리 확대 화면 패널 (LegZoomPanel)

    [Header("확대 화면 배경")]
    [SerializeField] private Sprite zoomBackdropSprite;

    [Header("확대 화면에 표시할 인벤토리 배경 복제본")]
    [SerializeField] private SpriteRenderer inventoryBackdropSource;
    [SerializeField] private RectTransform gameplayUiRoot;

    private static GameObject sharedBackdrop;
    private static GameObject sharedInventoryBackdrop;
    private GameObject backdrop;
    private Image inventoryBackdropImage;
    private RectTransform canvasRect;
    private RectTransform inventoryPanel;
    private Transform inventoryPanelOriginalParent;
    private int inventoryPanelOriginalSiblingIndex;
    private Vector2 inventoryPanelOriginalAnchorMin;
    private Vector2 inventoryPanelOriginalAnchorMax;
    private Vector2 inventoryPanelOriginalPivot;
    private Vector2 inventoryPanelOriginalAnchoredPosition;
    private Vector2 inventoryPanelOriginalSizeDelta;
    private Vector3 inventoryPanelOriginalScale;
    private Quaternion inventoryPanelOriginalRotation;
    private bool inventoryPanelRaised;

    private void Awake()
    {
        CacheInventoryPanelLayout();
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
            if (sharedInventoryBackdrop != null)
            {
                sharedInventoryBackdrop.SetActive(true);
                sharedInventoryBackdrop.transform.SetSiblingIndex(1);
            }
            zoomPanel.SetActive(true);
            SyncBackdrop();
        }
        else
        {
            Debug.LogWarning("LegZoomTrigger: zoomPanel이 연결되어 있지 않습니다!");
        }
    }

    private void CreateBackdrop()
    {
        if (zoomPanel == null || zoomBackdropSprite == null) return;

        Canvas canvas = zoomPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        canvas = canvas.rootCanvas;
        canvasRect = canvas.transform as RectTransform;

        if (sharedBackdrop != null)
        {
            backdrop = sharedBackdrop;
            CreateInventoryBackdropCopy(canvas.transform);
            return;
        }

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

        CreateInventoryBackdropCopy(canvas.transform);
    }

    private void CreateInventoryBackdropCopy(Transform canvasRoot)
    {
        if (inventoryBackdropSource == null || inventoryBackdropSource.sprite == null) return;

        if (sharedInventoryBackdrop != null)
        {
            inventoryBackdropImage = sharedInventoryBackdrop.GetComponent<Image>();
            return;
        }

        sharedInventoryBackdrop = new GameObject(
            "Leg Zoom Inventory Backdrop Copy",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = sharedInventoryBackdrop.GetComponent<RectTransform>();
        rect.SetParent(canvasRoot, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        inventoryBackdropImage = sharedInventoryBackdrop.GetComponent<Image>();
        inventoryBackdropImage.sprite = inventoryBackdropSource.sprite;
        inventoryBackdropImage.color = inventoryBackdropSource.color;
        inventoryBackdropImage.preserveAspect = false;
        inventoryBackdropImage.raycastTarget = false;

        // 시작 시에는 일반 게임 UI 아래에 둔다. 팝업 중에는 LegZoomPanel 위로 옮긴다.
        rect.SetSiblingIndex(1);
        sharedInventoryBackdrop.SetActive(false);
    }

    private void SyncBackdrop()
    {
        bool isOpen = zoomPanel != null && zoomPanel.activeInHierarchy;
        if (backdrop == null) return;

        if (backdrop.activeSelf != isOpen)
            backdrop.SetActive(isOpen);

        if (isOpen && backdrop.transform.GetSiblingIndex() != 0)
            backdrop.transform.SetAsFirstSibling();

        if (sharedInventoryBackdrop != null)
        {
            if (sharedInventoryBackdrop.activeSelf != isOpen)
                sharedInventoryBackdrop.SetActive(isOpen);

            if (isOpen)
            {
                if (sharedInventoryBackdrop.transform.parent != backdrop.transform.parent)
                    sharedInventoryBackdrop.transform.SetParent(backdrop.transform.parent, false);

                int panelIndex = zoomPanel.transform.GetSiblingIndex();
                sharedInventoryBackdrop.transform.SetSiblingIndex(panelIndex + 1);
                UpdateInventoryBackdropCopyRect();
                RaiseInventoryPanelAboveBackdropCopy();
            }
            else
            {
                RestoreInventoryPanelLayout();
                if (sharedInventoryBackdrop.transform.parent == backdrop.transform.parent)
                    sharedInventoryBackdrop.transform.SetSiblingIndex(1);
            }
        }
    }

    private void CacheInventoryPanelLayout()
    {
        if (gameplayUiRoot == null) return;

        inventoryPanel = gameplayUiRoot.Find("InventoryPanel") as RectTransform;
        if (inventoryPanel == null) return;

        inventoryPanelOriginalParent = inventoryPanel.parent;
        inventoryPanelOriginalSiblingIndex = inventoryPanel.GetSiblingIndex();
        inventoryPanelOriginalAnchorMin = inventoryPanel.anchorMin;
        inventoryPanelOriginalAnchorMax = inventoryPanel.anchorMax;
        inventoryPanelOriginalPivot = inventoryPanel.pivot;
        inventoryPanelOriginalAnchoredPosition = inventoryPanel.anchoredPosition;
        inventoryPanelOriginalSizeDelta = inventoryPanel.sizeDelta;
        inventoryPanelOriginalScale = inventoryPanel.localScale;
        inventoryPanelOriginalRotation = inventoryPanel.localRotation;
    }

    private void RaiseInventoryPanelAboveBackdropCopy()
    {
        if (inventoryPanel == null || canvasRect == null) return;

        if (!inventoryPanelRaised)
        {
            inventoryPanel.SetParent(canvasRect, true);
            inventoryPanelRaised = true;
        }

        inventoryPanel.SetSiblingIndex(sharedInventoryBackdrop.transform.GetSiblingIndex() + 1);
    }

    private void RestoreInventoryPanelLayout()
    {
        if (!inventoryPanelRaised || inventoryPanel == null || inventoryPanelOriginalParent == null)
            return;

        inventoryPanel.SetParent(inventoryPanelOriginalParent, false);
        inventoryPanel.anchorMin = inventoryPanelOriginalAnchorMin;
        inventoryPanel.anchorMax = inventoryPanelOriginalAnchorMax;
        inventoryPanel.pivot = inventoryPanelOriginalPivot;
        inventoryPanel.anchoredPosition = inventoryPanelOriginalAnchoredPosition;
        inventoryPanel.sizeDelta = inventoryPanelOriginalSizeDelta;
        inventoryPanel.localScale = inventoryPanelOriginalScale;
        inventoryPanel.localRotation = inventoryPanelOriginalRotation;
        inventoryPanel.SetSiblingIndex(inventoryPanelOriginalSiblingIndex);
        inventoryPanelRaised = false;
    }

    private void UpdateInventoryBackdropCopyRect()
    {
        if (inventoryBackdropSource == null || inventoryBackdropImage == null || canvasRect == null)
            return;

        Camera camera = Camera.main;
        if (camera == null) return;

        Bounds bounds = inventoryBackdropSource.bounds;
        float z = bounds.center.z;
        Vector3[] worldCorners =
        {
            new Vector3(bounds.min.x, bounds.min.y, z),
            new Vector3(bounds.min.x, bounds.max.y, z),
            new Vector3(bounds.max.x, bounds.min.y, z),
            new Vector3(bounds.max.x, bounds.max.y, z)
        };

        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        for (int i = 0; i < worldCorners.Length; i++)
        {
            Vector2 screenPoint = camera.WorldToScreenPoint(worldCorners[i]);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPoint, null, out Vector2 localPoint))
                return;

            min = Vector2.Min(min, localPoint);
            max = Vector2.Max(max, localPoint);
        }

        RectTransform rect = sharedInventoryBackdrop.GetComponent<RectTransform>();
        rect.anchoredPosition = (min + max) * 0.5f;
        rect.sizeDelta = max - min;
    }
}
