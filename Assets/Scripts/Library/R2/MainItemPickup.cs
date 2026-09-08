using UnityEngine;

public class MainItemPickup : MonoBehaviour
{
    public Sprite itemIcon;
    public SpriteRenderer backgroundRenderer;
    public Sprite newBackgroundSprite;

    [Header("아이템 획득 팝업")]
    public Sprite popupItemSprite;  // 획득 팝업창에 보여줄 이미지

    void OnMouseDown()
    {
        Debug.Log("노트북 클릭됨");
        bool added = InventoryManager.Instance.AddItem(itemIcon);
        if (added)
        {
            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }

            gameObject.SetActive(false);

            // 아이템 획득 팝업 표시
            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(popupItemSprite);
            }
        }
    }
}