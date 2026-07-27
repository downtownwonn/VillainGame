using System.Collections;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    public GameObject sitPlayer;
    public GameObject bendPlayer;
    public GameObject Canvas;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bendSound;

    bool changed = false;

    void Start()
    {
        bendPlayer.SetActive(false); // 시작하면 숙인 사람 숨김
        Canvas.SetActive(true);
    }

    void OnMouseDown()
    {
        if (changed) return;

        changed = true;
        StartCoroutine(ChangePlayerRoutine());
    }

    IEnumerator ChangePlayerRoutine()
    {
        // 효과음 먼저 재생
        if (audioSource != null && bendSound != null)
        {
            audioSource.PlayOneShot(bendSound);

            // 효과음 끝날 때까지 기다림
            yield return new WaitForSeconds(0.4f);
        }

        // 이미지 변경
        sitPlayer.SetActive(false);
        bendPlayer.SetActive(true);
    }
}