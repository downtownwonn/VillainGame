using UnityEngine;

public class LetterClickTrigger : MonoBehaviour
{
    [Header("편지 내용 패널")]
    public GameObject letterPanel;
    public SoundManager soundManager;

    void OnMouseDown()
    {
        if (letterPanel != null && !letterPanel.activeSelf)
        {
            if (soundManager != null) soundManager.PlayUIClick();
            HomeButtonBootstrap.TrackModalPanel(letterPanel);
            letterPanel.SetActive(true);
        }
    }
}
