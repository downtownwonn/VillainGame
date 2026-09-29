using UnityEngine;

public class LetterClickTrigger : MonoBehaviour
{
    [Header("편지 내용 패널")]
    public GameObject letterPanel;

    void OnMouseDown()
    {
        if (letterPanel != null)
        {
            letterPanel.SetActive(true);
        }
    }
}