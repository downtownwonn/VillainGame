using UnityEngine;
using UnityEngine.UI;

public class WarehouseLockDrop : MonoBehaviour
{
    public BackgroundManager backgroundManager;

    [Header("사운드 설정")]
    [Tooltip("문이 열릴 때/문 따는 소리 재생할 효과음 클립")]
    public AudioClip unlockSound; 

    private bool isUnlocked = false; // 이미 열쇠를 사용했는지 여부

    // DraggableItemofStore에서 드래그가 끝났을 때 이 함수를 호출해줍니다.
    public void OnItemDropped(DraggableItemofStore item)
    {
        if (isUnlocked) return; // 이미 사용된 경우 중복 실행 방지

        item.wasDropped = true;
        isUnlocked = true;

        // 🔊 1. 열쇠 사용/해제 소리 재생
        if (unlockSound != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, Camera.main.transform.position);
        }

        // 2. 인벤토리에서 열쇠(아이템) 이미지 비우기
        Image itemImage = item.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.color = new Color(1, 1, 1, 0);
        }

        // 3. 창고 오픈 권한 부여
        if (backgroundManager != null)
        {
            backgroundManager.UnlockWarehouse();
        }
    }

    // ★ 월드 오브젝트(PolygonCollider2D)를 마우스로 클릭했을 때 동작
    private void OnMouseDown()
    {
        if (backgroundManager != null)
        {
            backgroundManager.OpenWarehouse();
        }
    }
}