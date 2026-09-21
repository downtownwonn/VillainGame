using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProtagonistDropArea : MonoBehaviour
{
    [Header("부위별 장착 오브젝트 (GameObject)")]
    public GameObject headObject; // 머리 장착 오브젝트 (머리에 씌울 모자/수세미 등)
    public GameObject handObject; // 손 장착 오브젝트 (손에 쥘 장갑/봉 등)
    public GameObject bodyObject; // 몸통/안경 장착 오브젝트 (앞치마/안경 등)

    [Header("사운드 설정")]
    [Tooltip("옷이나 장비를 착용할 때 재생할 효과음")]
    public AudioClip equipSound;

    [Header("씬 전환 설정")]
    public Image fadeOverlay; // 화면 전체를 덮는 검은색 UI Image
    public string nextSceneName; // 이동할 씬 이름
    public float delayBeforeFade = 1.0f; // 슬롯 완료 후 대기 시간 (1초)
    public float fadeDuration = 1.0f; // 페이드 아웃에 걸리는 시간 (1초)

    private bool isTransitioning = false;

    // DraggableItemofStore에서 드래그가 끝났을 때 이 함수를 호출해줍니다.
    // 이렇게 변경
    public void OnItemDropped(DraggableItemofStore item)
    {
        if (isTransitioning) return;

        Image itemImage = item.GetComponent<Image>();
        if (itemImage == null || itemImage.sprite == null) return;

        string spriteName = itemImage.sprite.name;
        bool isEquippedSuccess = false;

        // 1. 머리 장착 조건
        if (spriteName.Contains("mophead") || spriteName.Contains("hat") || spriteName.Contains("Squ"))
        {
            if (headObject != null) headObject.SetActive(true);
            isEquippedSuccess = true;
        }
        // 2. 손 장착 조건
        else if (spriteName.Contains("glove") || spriteName.Contains("bong"))
        {
            if (handObject != null) handObject.SetActive(true);
            isEquippedSuccess = true;
        }
        // 3. 몸통 장착 조건
        else if (spriteName.Contains("apron") || spriteName.Contains("glasses"))
        {
            if (bodyObject != null) bodyObject.SetActive(true);
            isEquippedSuccess = true;
        }

        if (isEquippedSuccess)
        {
            if (equipSound != null)
            {
                AudioSource.PlayClipAtPoint(equipSound, Camera.main.transform.position);
            }

            item.wasDropped = true;
            item.ClearItem(); // 아이템 이미지를 비우고 원래 인벤토리 자리로 복귀

            CheckAllSlotsEquipped();
        }
    }
    // 3개 오브젝트가 모두 활성화되었는지 확인하는 메서드
    private void CheckAllSlotsEquipped()
    {
        bool isHeadDone = headObject == null || headObject.activeSelf;
        bool isHandDone = handObject == null || handObject.activeSelf;
        bool isBodyDone = bodyObject == null || bodyObject.activeSelf;

        if (isHeadDone && isHandDone && isBodyDone)
        {
            isTransitioning = true;
            StartCoroutine(FadeAndChangeSceneRoutine());
        }
    }

    // 1초 대기 -> Fade Out 코루틴
    private IEnumerator FadeAndChangeSceneRoutine()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);

            float timer = 0f;
            Color fadeColor = fadeOverlay.color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeColor.a = Mathf.Clamp01(timer / fadeDuration);
                fadeOverlay.color = fadeColor;
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next Scene Name이 지정되지 않았습니다!");
        }
    }
}