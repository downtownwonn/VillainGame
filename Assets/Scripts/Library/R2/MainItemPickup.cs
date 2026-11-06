using UnityEngine;

public class MainItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    public SpriteRenderer backgroundRenderer;
    public Sprite newBackgroundSprite;

    [Header("바뀐 배경 위치/크기 보정")]
    public Vector3 newBackgroundOffset;                 // 위치 이동량 (월드 단위)
    public Vector2 newBackgroundScale = Vector2.one;    // 크기 배율 (1 = 그대로)

    [Header("아이템 획득 팝업")]
    public Sprite popupItemSprite;

    [Header("효과음")]
    public AudioClip pickupSound;   // 인스펙터에서 효과음 파일 연결
    public AudioSource audioSource; // AudioManager의 AudioSource 연결

    void OnMouseDown()
    {
        bool added = InventoryManager.Instance.AddItem(itemData);
        if (added)
        {
            // 효과음 재생
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            if (backgroundRenderer != null && newBackgroundSprite != null)
            {
                backgroundRenderer.sprite = newBackgroundSprite;

                Transform bg = backgroundRenderer.transform;
                bg.localPosition += newBackgroundOffset; // 위치 보정

                Vector3 s = bg.localScale;               // 크기 보정 (기존 스케일에 곱함)
                bg.localScale = new Vector3(s.x * newBackgroundScale.x,
                                            s.y * newBackgroundScale.y,
                                            s.z);
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