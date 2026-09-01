using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeatherTipPickup : MonoBehaviour
{
    [Header("Objects")]
    public GameObject feather;
    public GameObject inventoryItem;

    [Header("Sounds")]
    public AudioSource featherDropSound;
    public AudioSource itemPickupSound;

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
        GetComponent<Collider2D>().enabled = false;

        // ① 깃털 빠지는 소리
        if (featherDropSound != null)
            featherDropSound.Play();

        // FeatherTip 숨기기
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.15f);

        // ② Feather 등장
        feather.SetActive(true);

        // 아이템 획득 효과음
        if (itemPickupSound != null)
            itemPickupSound.Play();

        // 2초 동안 보여주기
        yield return new WaitForSeconds(2f);

        // Feather 숨기기
        feather.SetActive(false);

        // 인벤토리에 깃털 추가
        if (inventoryItem != null)
        {
            Image featherImage = inventoryItem.GetComponent<Image>();

            if (featherImage != null)
            {
                InventoryManagerForCin.Instance.AddItem(featherImage.sprite);
            }
        }

        // 기존 인벤토리 아이콘 숨기기
        if (inventoryItem != null)
            inventoryItem.SetActive(false);

        // FeatherTip 완전히 제거
        gameObject.SetActive(false);
    }
}