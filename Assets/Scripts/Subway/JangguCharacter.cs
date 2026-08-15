using UnityEngine;
using System.Collections;

public class JangguCharacter : MonoBehaviour
{
    public GameObject jangguCharacter;
    public GameObject dance1;
    public GameObject dance2;

    public void StartDancing()
    {
        jangguCharacter.SetActive(false);
        dance1.SetActive(true);
        dance2.SetActive(false);

        StartCoroutine(DanceAnimation());
    }

    private IEnumerator DanceAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            dance1.SetActive(false);
            dance2.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            dance2.SetActive(false);
            dance1.SetActive(true);
        }
    }

    public void ReceiveSangmo()
    {
        StartDancing();
    }
}