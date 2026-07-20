using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeatherPuzzleController : MonoBehaviour
{
    [Header("Objects")]
    public RectTransform featherUI;
    public BoxCollider2D shoeCollider;
    public ShoeAnimation shoeAnimation;

    [Header("Settings")]
    public float holdTime = 1f;

    private Camera cam;
    private bool started = false;
    private Coroutine holdRoutine;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (started)
            return;

        // Feather가 활성화 안되어있으면 검사 안함
        if (!featherUI.gameObject.activeInHierarchy)
            return;

        // 마우스가 안눌려있으면 취소
        if (!Input.GetMouseButton(0))
        {
            CancelHold();
            return;
        }

        // Feather 화면좌표 → 월드좌표
        Vector3 world =
            cam.ScreenToWorldPoint(featherUI.position);

        world.z = 0;

        bool onShoe =
            shoeCollider.OverlapPoint(world);

        if (onShoe)
        {
            if (holdRoutine == null)
                holdRoutine = StartCoroutine(Hold());
        }
        else
        {
            CancelHold();
        }
    }

    IEnumerator Hold()
    {
        yield return new WaitForSeconds(holdTime);

        started = true;

        StartCoroutine(FeatherAnimation());
    }

    IEnumerator FeatherAnimation()
    {
        Vector2 start = featherUI.anchoredPosition;

        float amount = 15f;

        for (int i = 0; i < 6; i++)
        {
            featherUI.anchoredPosition =
                start + Vector2.left * amount;

            yield return new WaitForSeconds(0.06f);

            featherUI.anchoredPosition =
                start + Vector2.right * amount;

            yield return new WaitForSeconds(0.06f);
        }

        featherUI.anchoredPosition = start;

        shoeAnimation.StartPuzzle();
    }

    void CancelHold()
    {
        if (holdRoutine != null)
        {
            StopCoroutine(holdRoutine);
            holdRoutine = null;
        }
    }
}