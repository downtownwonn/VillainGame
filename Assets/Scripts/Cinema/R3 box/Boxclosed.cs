using System.Collections;
using UnityEngine;

public class OpenClosedBox : MonoBehaviour
{
    public GameObject closedBox;
    public GameObject openedBox;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;

    private bool opened = false;
    private bool canOpen = false;

    // 사람이 숙인 후 ChangePlayer에서 호출
    public void EnableBoxClick()
    {
        canOpen = true;

        Debug.Log("조명 조절기 박스 클릭 가능!");
    }

    void OnMouseDown()
    {
        // 사람이 숙이기 전에는 클릭해도 아무것도 하지 않음
        if (!canOpen)
        {
            Debug.Log("아직 박스를 열 수 없습니다.");
            return;
        }

        // 이미 열렸으면 무시
        if (opened)
            return;

        opened = true;
        StartCoroutine(OpenBoxRoutine());
    }

    IEnumerator OpenBoxRoutine()
    {
        // 효과음 재생
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);

            yield return new WaitForSeconds(0.8f);
        }

        // 상자 열기
        closedBox.SetActive(false);
        openedBox.SetActive(true);
    }
}