using UnityEngine;
using UnityEngine.UI;

public class JarvanVillain : MonoBehaviour
{
    public bool UseBodyguards(Image bodyguardImage)
    {
        if (bodyguardImage == null)
            return false;

        InventoryItemData data =
            bodyguardImage.GetComponent<InventoryItemData>();

        if (data == null)
        {
            Debug.Log("아이템 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (data.itemType != ItemType.Bodyguards)
        {
            Debug.Log("이 아이템으로는 자르반을 물리칠 수 없습니다.");
            return false;
        }

        Debug.Log("경호원 3인조를 자르반에게 사용했습니다!");
        Debug.Log("R3 CLEAR!");

        // 경호원 아이템 소모
        bodyguardImage.sprite = null;
        bodyguardImage.enabled = false;

        // 여기서 나중에 클리어 연출 추가 가능
        return true;
    }
}