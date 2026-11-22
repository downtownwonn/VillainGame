using UnityEngine;

public class BodyguardGroupManager : MonoBehaviour
{
    public BodyguardCharacter[] bodyguards;
    public Sprite bodyguardItemSprite;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Item Get Popup")]
    [SerializeField] private Sprite popupItemSprite;

    [Header("Sound")]
    [SerializeField] private AudioSource soundEffectsSource;
    [SerializeField] private AudioClip sunglassesEquipSound;
    [SerializeField] private AudioClip bodyguardRevealSound;

    private int sunglassesCount = 0;
    private bool groupCollected = false;


    public void SunglassesCompleted(BodyguardCharacter character)
    {
        sunglassesCount++;

        Debug.Log(
            "선글라스 착용 인원: " +
            sunglassesCount + " / " +
            bodyguards.Length
        );

        // 앞의 두 명에게만 착용 효과음을 재생하고, 세 번째 완료 때는 TADA만 재생합니다.
        if (sunglassesCount < bodyguards.Length &&
            soundEffectsSource != null && sunglassesEquipSound != null)
        {
            soundEffectsSource.PlayOneShot(sunglassesEquipSound);
        }

        if (sunglassesCount >= bodyguards.Length)
        {
            MakeAllBodyguards();
        }
    }


    private void MakeAllBodyguards()
    {
        Debug.Log("직장인 3명 모두 선글라스 착용! 경호원으로 변신!");

        foreach (BodyguardCharacter bodyguard in bodyguards)
        {
            if (bodyguard != null)
            {
                bodyguard.BecomeBodyguard();
            }
        }

        if (soundEffectsSource != null && bodyguardRevealSound != null)
        {
            soundEffectsSource.PlayOneShot(bodyguardRevealSound);
        }
    }


    public void CollectBodyguards()
    {
        if (groupCollected)
            return;

        groupCollected = true;

        // 경호원 3명 모두 숨기기
        foreach (BodyguardCharacter bodyguard in bodyguards)
        {
            if (bodyguard != null)
            {
                bodyguard.gameObject.SetActive(false);
            }
        }

        // 경호원 3인조 아이템을 인벤토리에 추가
        if (SubwayInventoryManager.Instance != null)
        {
            if (itemData == null)
            {
                Debug.LogError(
                    gameObject.name +
                    "에 경호원 3인조 ItemData가 연결되지 않았습니다."
                );
                return;
            }

            SubwayInventoryManager.Instance.AddItem(
                itemData,
                ItemType.Bodyguards
            );

            // 아이템 획득 팝업
            if (ItemGetPopup.Instance != null)
            {
                ItemGetPopup.Instance.Show(popupItemSprite);
            }
        }

        Debug.Log("경호원 3인조 획득!");
    }
}
