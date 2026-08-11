using UnityEngine;

public class OpenBoxUI : MonoBehaviour
{
    [Header("Puzzle")]
    public CinemaLightPuzzleController puzzleController; // GameManager 오브젝트를 여기 드래그

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip PanelSound;

    void OnMouseDown()
    {
        puzzleController.OpenPuzzle();
        audioSource.PlayOneShot(PanelSound);
    }
}