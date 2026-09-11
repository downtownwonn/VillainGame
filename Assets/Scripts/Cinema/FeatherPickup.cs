using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeatherTipPickup : MonoBehaviour
{
    [Header("Objects")]
    public GameObject feather;
    public GameObject inventoryItem;

    [Header("Inventory")]
    [SerializeField] private Sprite featherSprite;

    [Header("Item Get Popup")]
    public Sprite popupItemSprite;
    public float popupDuration = 1.5f;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Sounds")]
    public AudioSource featherDropSound;

    private float lastClickTime = 0f;
    private const float doubleClickDelay = 0.3f;

    private bool pickedUp = false;

    void OnMouseDown()
    {
        if (pickedUp) return;

        if (Time.time - lastClickTime < doubleClickDelay)
        {
            pickedUp = true;
            StartCoroutine(PickupSequence());
        }

        lastClickTime = Time.time;
    }

    IEnumerator PickupSequence()
    {
        // 더 이상 클릭되지 않도록 콜라이더 끄기
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        // 깃털 빠지는 소리
        if (featherDropSound != null)
            featherDropSound.Play();

        // FeatherTip 숨기기
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.15f);

        // Feather 등장
        if (feather != null)
            feather.SetActive(true);

        // 인벤토리에 깃털 추가// 인벤토리에 깃털 추가
        if (itemData != null)
        {
            Debug.Log("깃털 AddItem 실행: " + itemData.itemName);

            bool result =
                InventoryManagerForCin.Instance.AddItem(
                    itemData,
                    CinemaItemType.Feather
                );

            Debug.Log("깃털 AddItem 결과: " + result);
        }
        else
        {
            Debug.LogError("FeatherTipPickup: itemData가 비어 있습니다!");
        }

        // 획득 팝업 표시
        if (ItemGetPopup.Instance == null)
        {
            Debug.LogWarning("FeatherTipPickup: ItemGetPopup Instance를 찾을 수 없습니다.");
        }
        else if (popupItemSprite == null)
        {
            Debug.LogWarning("FeatherTipPickup: Popup Item Sprite가 설정되지 않았습니다.");
        }
        else
        {
            Debug.Log("FeatherTipPickup: 획득 팝업을 표시합니다.");
            ItemGetPopup.Instance.Show(popupItemSprite, popupDuration);
        }

        // Feather 숨기기
        if (feather != null)
            feather.SetActive(false);

        // FeatherTip 완전히 제거
        gameObject.SetActive(false);
    }
}