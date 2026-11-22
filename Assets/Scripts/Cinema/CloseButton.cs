using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panel;
    public SoundManager soundManager;

    public void Close()
    {
        if (panel != null && soundManager != null)
            soundManager.PlayObjectClick();

        HomeButtonBootstrap.SetModalPopupOpen(panel, false);
        panel.SetActive(false);
    }
}
