using UnityEngine;

public class BookPickup : MonoBehaviour
{
    public Sprite bookIcon;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(bookIcon);
        if (added)
        {
            gameObject.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        Color c = sr.color;
        c.a = 0.35f; // 마우스 올리면 살짝 보이게 (원하는 만큼 조정 가능)
        sr.color = c;
    }

    void OnMouseExit()
    {
        Color c = sr.color;
        c.a = 0f; // 마우스 떼면 다시 완전히 투명하게
        sr.color = c;
    }
}