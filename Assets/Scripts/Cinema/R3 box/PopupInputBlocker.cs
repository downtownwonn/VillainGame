using System.Collections.Generic;
using UnityEngine;

public class PopupInputBlocker : MonoBehaviour
{
    private List<Collider2D> disabledColliders = new List<Collider2D>();

    private void OnEnable()
    {
        BlockGameColliders();
    }

    private void OnDisable()
    {
        RestoreGameColliders();
    }

    private void BlockGameColliders()
    {
        disabledColliders.Clear();

        Collider2D[] colliders = FindObjectsByType<Collider2D>(
            FindObjectsInactive.Exclude
        );

        foreach (Collider2D col in colliders)
        {
            if (col == null)
                continue;

            // 현재 활성화되어 있는 Collider만 저장 후 끔
            if (col.enabled)
            {
                disabledColliders.Add(col);
                col.enabled = false;
            }
        }

        Debug.Log("팝업 열림 → 게임 Collider 전체 잠금");
    }

    private void RestoreGameColliders()
    {
        foreach (Collider2D col in disabledColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        disabledColliders.Clear();

        Debug.Log("팝업 닫힘 → 게임 Collider 복구");
    }
}