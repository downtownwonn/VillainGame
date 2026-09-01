using UnityEngine;
using System.Collections;

public class JangguCharacter : MonoBehaviour
{
    public GameObject jangguCharacter;
    public GameObject dance1;
    public GameObject dance2;

    private bool isDancing = false;
    private Coroutine danceCoroutine;


    public void StartDancing()
    {
        if (isDancing)
            return;

        isDancing = true;

        jangguCharacter.SetActive(false);
        dance1.SetActive(true);
        dance2.SetActive(false);

        StartDanceCoroutine();
    }


    private void StartDanceCoroutine()
    {
        if (danceCoroutine != null)
        {
            StopCoroutine(danceCoroutine);
        }

        danceCoroutine = StartCoroutine(DanceAnimation());
    }


    private IEnumerator DanceAnimation()
    {
        while (isDancing)
        {
            dance1.SetActive(true);
            dance2.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            dance1.SetActive(false);
            dance2.SetActive(true);

            yield return new WaitForSeconds(0.5f);
        }
    }


    private void OnEnable()
    {
        if (isDancing)
        {
            dance1.SetActive(true);
            dance2.SetActive(false);

            StartDanceCoroutine();
        }
    }


    public void ReceiveSangmo()
    {
        StartDancing();
    }
}