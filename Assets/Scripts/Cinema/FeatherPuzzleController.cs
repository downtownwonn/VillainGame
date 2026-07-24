using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



/// <summary>
/// Feather puzzle controller.
/// Requires:
/// - DraggableItem with OnDragEnded event and ReturnToOriginalPosition().
/// - UI Feather(Image)
/// - FeatherEffect(Sprite, inactive at start)
/// - Shoe(Sprite + BoxCollider2D)
/// - TicklePoint(empty child on shoe)
/// - Person(Sprite/GameObject)
/// - Inventory shoe(Image, inactive)
/// </summary>
public class FeatherPuzzleController : MonoBehaviour
{
    [Header("Fade")]
    public Image fadePanel;
    public float fadeTime = 1.6f;

    [Header("Next Scene")]
    public string nextSceneName;

    [Header("References")]
    public DraggableItem featherUI;
    public Collider2D shoeCollider;
    public Transform shoeTransform;
    public Transform ticklePoint;
    public GameObject featherEffect;
    public GameObject personObject;
    public GameObject inventoryShoe;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tickleSound;
    public AudioClip dropSound;
    public AudioClip walkSound;

    [Header("Animation")]
    public float flyTime = 0.2f;
    public float tickleTime = 2.0f;
    public float featherRadius = 0.08f;
    public float featherRotation = 22f;
    public float shoeRotate = 6f;
    public float shoeBounce = 0.03f;
    public float dropDistance = 4f;
    public float dropSpeed = 2.5f;

    Camera cam;
    bool solved;
    Vector3 shoeStartPos;
    Quaternion shoeStartRot;

    void Start()
    {
        cam = Camera.main;
        featherUI.OnDragEnded += OnFeatherDropped;
        if (featherEffect) featherEffect.SetActive(false);
        if (inventoryShoe) inventoryShoe.SetActive(false);
    }

    void OnDestroy()
    {
        if (featherUI != null)
            featherUI.OnDragEnded -= OnFeatherDropped;
    }

    void OnFeatherDropped(PointerEventData e)
    {
        if (solved) return;

        Vector3 p = cam.ScreenToWorldPoint(e.position);
        p.z = 0;

        if (!shoeCollider.OverlapPoint(p))
            return;

        solved = true;
        featherUI.wasDropped = true;
        StartCoroutine(PuzzleRoutine());
    }

    IEnumerator PuzzleRoutine()
    {
        featherUI.gameObject.SetActive(false);

        featherEffect.SetActive(true);

        Vector3 start = cam.ScreenToWorldPoint(Input.mousePosition);
        start.z = 0;
        featherEffect.transform.position = start;

        shoeStartPos = shoeTransform.position;
        shoeStartRot = shoeTransform.rotation;

        float t = 0;
        while (t < flyTime)
        {
            t += Time.deltaTime;
            featherEffect.transform.position =
                Vector3.Lerp(start, ticklePoint.position, t / flyTime);
            yield return null;
        }

        if (audioSource && tickleSound)
            audioSource.PlayOneShot(tickleSound);

        t = 0;
        while (t < tickleTime)
        {
            t += Time.deltaTime;

            Vector3 offset = new Vector3(
                Mathf.Sin(t * 25f) * featherRadius,
                Mathf.Cos(t * 50f) * featherRadius * 0.4f,
                0);

            featherEffect.transform.position = ticklePoint.position + offset;

            featherEffect.transform.rotation =
                Quaternion.Euler(0, 0, Mathf.Sin(t * 22f) * featherRotation);

            shoeTransform.rotation =
                Quaternion.Euler(0, 0, Mathf.Sin(t * 18f) * shoeRotate);

            shoeTransform.position =
                shoeStartPos +
                new Vector3(0, Mathf.Abs(Mathf.Sin(t * 18f)) * shoeBounce, 0);

            yield return null;
        }

        shoeTransform.rotation = shoeStartRot;
        shoeTransform.position = shoeStartPos;

        if (audioSource && dropSound)
            audioSource.PlayOneShot(dropSound);

        Vector3 end = shoeStartPos + Vector3.down * dropDistance;

        bool personGone = false;

        float d = 0;
        while (d < 1)
        {
            d += Time.deltaTime * dropSpeed;
            float eased = d * d;
            shoeTransform.position = Vector3.Lerp(shoeStartPos, end, eased);
            // 낙하가 절반쯤 진행되면
            if (!personGone && d >= 0.2f)
            {
                personGone = true;

                if (personObject)
                    personObject.SetActive(false);

                if (audioSource && walkSound)
                    audioSource.PlayOneShot(walkSound);
            }

            yield return null;
        }

        shoeTransform.gameObject.SetActive(false);

        featherEffect.SetActive(false);

        featherUI.ReturnToOriginalPosition();

        // 다음 씬으로 이동 시작
        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        // 발걸음 소리 끝날 때까지 기다림
        if (walkSound != null)
            if (walkSound != null)
                yield return new WaitForSeconds(Mathf.Max(0, walkSound.length - fadeTime-0.5f));

        float t = 0;

        Color c = fadePanel.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(0, 1, t / fadeTime);

            fadePanel.color = c;

            yield return null;
        }

        SceneManager.LoadScene("CinemaR1Clear");
    }
}
