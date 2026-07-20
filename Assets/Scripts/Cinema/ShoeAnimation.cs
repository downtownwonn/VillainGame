using System.Collections;
using UnityEngine;

public class ShoeAnimation : MonoBehaviour
{
    [Header("Objects")]
    public GameObject inventoryShoe;

    [Header("Audio")]
    public AudioSource tickleSound;
    public AudioSource fallSound;
    public AudioSource pickupSound;

    [Header("Shake")]
    public float shakeTime = 0.8f;
    public float shakeAmount = 0.06f;

    [Header("Fall")]
    public float fallDistance = 4f;
    public float fallSpeed = 5f;
    public float rotateSpeed = 180f;

    private bool isPlaying = false;

    public void StartPuzzle()
    {
        if (isPlaying)
            return;

        StartCoroutine(PuzzleRoutine());
    }

    IEnumerator PuzzleRoutine()
    {
        isPlaying = true;

        //-----------------------------
        // 1. 간지러운 효과음
        //-----------------------------
        if (tickleSound != null)
            tickleSound.Play();

        //-----------------------------
        // 2. 신발 흔들기
        //-----------------------------
        Vector3 startPos = transform.position;

        float timer = 0;

        while (timer < shakeTime)
        {
            timer += Time.deltaTime;

            float x = Mathf.Sin(timer * 40f) * shakeAmount;

            transform.position = startPos + new Vector3(x, 0, 0);

            yield return null;
        }

        transform.position = startPos;

        if (tickleSound != null)
            tickleSound.Stop();

        //-----------------------------
        // 3. 툭 효과음
        //-----------------------------
        if (fallSound != null)
            fallSound.Play();

        //-----------------------------
        // 4. 떨어지기
        //-----------------------------
        Vector3 targetPos =
            startPos + Vector3.down * fallDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                fallSpeed * Time.deltaTime);

            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        //-----------------------------
        // 5. 화면에서 제거
        //-----------------------------
        gameObject.SetActive(false);

        //-----------------------------
        // 6. 인벤토리 Shoe 활성화
        //-----------------------------
        inventoryShoe.SetActive(true);

        //-----------------------------
        // 7. 아이템 획득 효과음
        //-----------------------------
        if (pickupSound != null)
            pickupSound.Play();

        isPlaying = false;
    }
}