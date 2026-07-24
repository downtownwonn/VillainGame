using UnityEngine;

public class MainItemPickup : MonoBehaviour
{
    public Sprite itemIcon;              // 인벤토리에 담길 아이콘
    public SpriteRenderer backgroundRenderer; // 바뀔 배경의 Sprite Renderer
    public Sprite newBackgroundSprite;   // 바뀐 후의 배경 이미지

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(itemIcon);
        if (added)
        {
            // 배경 이미지 교체
            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;
            }

            // 클릭 영역(하이라이트)은 사라지게
            gameObject.SetActive(false);
        }
    }
}
