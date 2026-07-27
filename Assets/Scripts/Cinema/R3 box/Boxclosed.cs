using System.Collections;
using UnityEngine;

public class OpenClosedBox : MonoBehaviour
{
    public GameObject closedBox;
    public GameObject openedBox;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;

    bool opened = false;

    void OnMouseDown()
    {
        if (opened) return;

        opened = true;
        StartCoroutine(OpenBoxRoutine());
    }

    IEnumerator OpenBoxRoutine()
    {
        // 효과음 재생
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);

            // 효과음이 끝날 때까지 대기
            yield return new WaitForSeconds(0.8f);
        }

        // 상자 열기
        closedBox.SetActive(false);
        openedBox.SetActive(true);
    }
}