using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundIntroManager : MonoBehaviour
{
    public GameObject roundPanel;
    public GameObject gameplayUI;   // 추가

    public Image panelImage;
    public TextMeshProUGUI roundText;

    void Start()
    {
        StartCoroutine(ShowRoundPanel());
    }

    IEnumerator ShowRoundPanel()
    {
        // 인트로 시작
        gameplayUI.SetActive(false);
        roundPanel.SetActive(true);

        Color bg = panelImage.color;
        Color txt = roundText.color;

        bg.a = 1f;
        txt.a = 1f;

        panelImage.color = bg;
        roundText.color = txt;

        yield return new WaitForSeconds(2f);

        // 텍스트 페이드
        float t = 0f;
        float textFadeTime = 0.8f;

        while (t < textFadeTime)
        {
            t += Time.deltaTime;
            txt.a = Mathf.Lerp(1f, 0f, t / textFadeTime);
            roundText.color = txt;
            yield return null;
        }

        // 배경 페이드
        t = 0f;
        float bgFadeTime = 1.6f;

        while (t < bgFadeTime)
        {
            t += Time.deltaTime;
            bg.a = Mathf.Lerp(1f, 0f, t / bgFadeTime);
            panelImage.color = bg;
            yield return null;
        }

        roundPanel.SetActive(false);
        gameplayUI.SetActive(true);   // 다시 보여주기
    }
}