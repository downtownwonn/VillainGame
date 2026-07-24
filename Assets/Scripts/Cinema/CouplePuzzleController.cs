using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Couple puzzle controller.
/// Shoe drag → Drop on couple → Shoe falls → Vomit sound
/// → Couple fade out → Walk sound → Fade → Next Scene
/// </summary>
public class CouplePuzzleController : MonoBehaviour
{
    [Header("Fade")]
    public Image fadePanel;
    public float fadeTime = 1.6f;

    [Header("Next Scene")]
    public string nextSceneName;

    [Header("References")]
    public DraggableItem shoeUI;
    public Collider2D coupleCollider;
    public Transform dropPoint;
    public GameObject shoeEffect;
    public SpriteRenderer coupleSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shoeDropSound;
    public AudioClip vomitSound;
    public AudioClip walkSound;

    [Header("Animation")]
    public float flyTime = 0.25f;
    public float bounceHeight = 0.18f;
    public float bounceTime = 0.18f;
    public float fadeOutTime = 1.2f;

    Camera cam;
    bool solved;

    void Start()
    {
        cam = Camera.main;

        shoeUI.OnDragEnded += OnShoeDropped;

        if (shoeEffect)
            shoeEffect.SetActive(false);
    }

    void OnDestroy()
    {
        if (shoeUI != null)
            shoeUI.OnDragEnded -= OnShoeDropped;
    }

    void OnShoeDropped(PointerEventData e)
    {
        if (solved)
            return;

        Vector3 world =
            cam.ScreenToWorldPoint(e.position);

        world.z = 0;

        if (!coupleCollider.OverlapPoint(world))
            return;

        solved = true;

        shoeUI.wasDropped = true;

        StartCoroutine(PuzzleRoutine());
    }

    IEnumerator PuzzleRoutine()
    {
        shoeUI.gameObject.SetActive(false);

        shoeEffect.SetActive(true);

        Vector3 start =
            cam.ScreenToWorldPoint(Input.mousePosition);

        start.z = 0;

        shoeEffect.transform.position = start;

        //----------------------------------
        // Shoe Fly
        //----------------------------------

        float t = 0;

        while (t < flyTime)
        {
            t += Time.deltaTime;

            shoeEffect.transform.position =
                Vector3.Lerp(
                    start,
                    dropPoint.position,
                    t / flyTime);

            yield return null;
        }

        //----------------------------------
        // Drop Sound
        //----------------------------------

        if (audioSource && shoeDropSound)
            audioSource.PlayOneShot(shoeDropSound);

        //----------------------------------
        // Bounce
        //----------------------------------

        Vector3 basePos = dropPoint.position;

        t = 0;

        while (t < bounceTime)
        {
            t += Time.deltaTime;

            float p = t / bounceTime;

            float h =
                Mathf.Sin(p * Mathf.PI)
                * bounceHeight;

            shoeEffect.transform.position =
                basePos + Vector3.up * h;

            yield return null;
        }

        shoeEffect.transform.position = basePos;

        //----------------------------------
        // Vomit Sound
        //----------------------------------

        if (audioSource && vomitSound)
            audioSource.PlayOneShot(vomitSound);

        yield return new WaitForSeconds(0.4f);

        //----------------------------------
        // Couple Fade Out
        //----------------------------------

        Color c = coupleSprite.color;

        t = 0;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;

            c.a =
                Mathf.Lerp(
                    1,
                    0,
                    t / fadeOutTime);

            coupleSprite.color = c;

            yield return null;
        }

        coupleSprite.gameObject.SetActive(false);

        //==============================
        // Fade & Scene Load
        //==============================

        StartCoroutine(FadeAndLoadScene());
    }


    IEnumerator FadeAndLoadScene()
    {
        // 걸어가는 소리가 끝날 때까지 대기
        if (walkSound != null)
        {
            yield return new WaitForSeconds(
                Mathf.Max(0, walkSound.length - fadeTime)
            );
        }


        float t = 0;

        Color c = fadePanel.color;


        while (t < fadeTime)
        {
            t += Time.deltaTime;

            c.a =
                Mathf.Lerp(
                    0,
                    1,
                    t / fadeTime
                );

            fadePanel.color = c;

            yield return null;
        }


        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}