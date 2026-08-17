using UnityEngine;
using UnityEngine.UI;

public class SubwayInventoryManager : MonoBehaviour
{
    public static SubwayInventoryManager Instance;

    public Image[] itemIcons;
    public Sprite scissorsSprite;

    private void Awake()
    {
        Instance = this;

        foreach (Image img in itemIcons)
        {
            img.sprite = null;
            img.enabled = false;
        }
    }

    public void AddItem(Sprite sprite, ItemType itemType)
    {
        for (int i = 0; i < itemIcons.Length; i++)
        {
            if (!itemIcons[i].enabled)
            {
                itemIcons[i].sprite = sprite;
                itemIcons[i].preserveAspect = true;
                itemIcons[i].enabled = true;

                InventoryItemData data =
                    itemIcons[i].GetComponent<InventoryItemData>();

                if (data != null)
                {
                    data.itemType = itemType;

                    Debug.Log(
                        "인벤토리 슬롯 " + i +
                        "에 들어간 아이템 타입 = " + data.itemType
                    );
                }
                else
                {
                    Debug.LogError(
                        "인벤토리 슬롯 " + i +
                        "에 InventoryItemData가 없습니다!"
                    );
                }

                return;
            }
        }
    }

    public bool TryCombine(Image first, Image second)
    {
        InventoryItemData firstData =
            first.GetComponent<InventoryItemData>();

        InventoryItemData secondData =
            second.GetComponent<InventoryItemData>();

        if (firstData == null || secondData == null)
        {
            Debug.LogError("InventoryItemData가 없습니다.");
            return false;
        }

        bool canCombine =
            (firstData.itemType == ItemType.HandleLeft &&
             secondData.itemType == ItemType.HandleRight)
            ||
            (firstData.itemType == ItemType.HandleRight &&
             secondData.itemType == ItemType.HandleLeft);

        if (!canCombine)
        {
            Debug.Log("합칠 수 없는 아이템입니다.");
            return false;
        }

        Debug.Log("손잡이 2개를 합쳐 가위 생성!");
        Debug.Log("scissorsSprite = " + scissorsSprite);

        // 드래그해서 도착한 슬롯을 가위로 변경
        second.sprite = scissorsSprite;
        second.preserveAspect = true;
        second.enabled = true;

        secondData.itemType = ItemType.Scissors;

        // 드래그한 원래 슬롯은 비우기
        first.sprite = null;
        first.enabled = false;

        firstData.itemType = default;

        // 드래그한 아이콘을 원래 슬롯으로 복귀
        SubwayDraggableItem draggable =
            first.GetComponent<SubwayDraggableItem>();

        if (draggable != null)
        {
            draggable.ReturnToOriginalPosition();
        }

        return true;
    }
}