using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panel;

    public void Close()
    {
        HomeButtonBootstrap.SetModalPopupOpen(panel, false);
        panel.SetActive(false);
    }
}
