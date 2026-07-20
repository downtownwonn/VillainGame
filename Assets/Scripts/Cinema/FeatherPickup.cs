using System.Collections;
using UnityEngine;

public class FeatherTipPickup : MonoBehaviour
{
    [Header("Objects")]
    public GameObject feather;          // PaddingNPC 아래 Feather
    public GameObject inventoryItem;    // 인벤토리 Feather

    [Header("Sounds")]
    public AudioSource featherDropSound;    // 깃털 떨어지는 소리
    public AudioSource itemPickupSound;     // 아이템 획득 소리

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

        // FeatherTip 숨기기 (오브젝트는 비활성화하지 않음!!)
        GetComponent<SpriteRenderer>().enabled = false;

        // 살짝 텀
        yield return new WaitForSeconds(0.15f);

        // ② Feather 등장
        feather.SetActive(true);

        // 아이템 획득 효과음
        if (itemPickupSound != null)
            itemPickupSound.Play();

        // 1초 동안 보여주기
        yield return new WaitForSeconds(2f);

        // Feather 숨기기
        feather.SetActive(false);

        // 인벤토리 활성화
        inventoryItem.SetActive(true);

        // 이제 FeatherTip 오브젝트 완전히 제거
        gameObject.SetActive(false);
    }
}