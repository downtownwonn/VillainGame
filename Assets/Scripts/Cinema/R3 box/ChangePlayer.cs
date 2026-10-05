using System.Collections;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    public GameObject sitPlayer;
    public GameObject bendPlayer;
    public GameObject Canvas;

    [Header("Box")]
    public OpenClosedBox openBox;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bendSound;

    private bool changed = false;

    void Start()
    {
        // 시작하면 숙인 사람 숨김
        bendPlayer.SetActive(false);

        Canvas.SetActive(true);
    }

    void OnMouseDown()
    {
        if (changed)
            return;

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

        // 사람 자세 변경
        sitPlayer.SetActive(false);
        bendPlayer.SetActive(true);

        // 사람이 숙인 후에만 박스 클릭 가능
        if (openBox != null)
        {
            openBox.EnableBoxClick();
        }
    }
}