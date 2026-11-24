using UnityEngine;

public class DansoVillain : MonoBehaviour
{
    [Tooltip("Enable only on the R2 main protagonist pose that accepts the janggu and clears the stage.")]
    public bool isR2ClearTarget;

    public void ReceiveJanggu()
    {
        Debug.Log("주인공에게 장구 전달 성공! R2 CLEAR!");

        // 여기서 나중에 클리어 연출을 넣으면 됨
    }
}
