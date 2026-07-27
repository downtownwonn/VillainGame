using UnityEngine;

public class OpenBoxUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip PanelSound;


    void OnMouseDown()
    {
        panel.SetActive(true);

        audioSource.PlayOneShot(PanelSound);

    }
}