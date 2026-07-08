using System.Collections;
using UnityEngine;

public class RoundIntroManager : MonoBehaviour
{
    public GameObject roundPanel;

    void Start()
    {
        StartCoroutine(ShowRoundPanel());
    }

    IEnumerator ShowRoundPanel()
    {
        roundPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        roundPanel.SetActive(false);

        // 여기부터 게임 시작
    }
}