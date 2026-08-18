using UnityEngine;
using UnityEngine.UI;

public class BackpackVillainTarget : MonoBehaviour
{
    public bool UseScissors(Image scissorsImage)
    {
        if (scissorsImage == null)
            return false;

        InventoryItemData data =
            scissorsImage.GetComponent<InventoryItemData>();

        if (data == null)
        {
            Debug.Log("아이템 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (data.itemType != ItemType.Scissors)
        {
            Debug.Log("이 아이템으로는 백팩을 사용할 수 없습니다.");
            return false;
        }

        Debug.Log("가위를 백팩에 사용했습니다!");

        // 가위 사용
        scissorsImage.sprite = null;
        scissorsImage.enabled = false;

        // 나중에 여기에서 참교육 애니메이션 재생
        // Animator를 만들면 이 부분에 넣으면 됨.

        return true;
    }
}