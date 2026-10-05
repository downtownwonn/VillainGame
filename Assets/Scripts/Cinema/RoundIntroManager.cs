using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundIntroManager : MonoBehaviour
{
    public GameObject roundPanel;
    public GameObject gameplayUI;

    public Image panelImage;
    public TextMeshProUGUI roundText;

    private List<Collider2D> gameplayColliders = new List<Collider2D>();

    void Start()
    {
        StartCoroutine(ShowRoundPanel());
    }

    IEnumerator ShowRoundPanel()
    {
        // 게임 Collider 전부 잠금
        LockGameplayColliders();

        gameplayUI.SetActive(false);
        roundPanel.SetActive(true);

        Color bg = panelImage.color;
        Color txt = roundText.color;

        bg.a = 1f;
        txt.a = 1f;

        panelImage.color = bg;
        roundText.color = txt;

        yield return new WaitForSeconds(2f);

        float t = 0f;
        float textFadeTime = 0.8f;

        while (t < textFadeTime)
        {
            t += Time.deltaTime;
            txt.a = Mathf.Lerp(1f, 0f, t / textFadeTime);
            roundText.color = txt;
            yield return null;
        }

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
        gameplayUI.SetActive(true);

        // 게임 Collider 다시 활성화
        UnlockGameplayColliders();
    }

    void LockGameplayColliders()
    {
        gameplayColliders.Clear();

        Collider2D[] colliders = FindObjectsByType<Collider2D>(
            FindObjectsInactive.Exclude
        );

        foreach (Collider2D col in colliders)
        {
            if (col.enabled)
            {
                gameplayColliders.Add(col);
                col.enabled = false;
            }
        }
    }

    void UnlockGameplayColliders()
    {
        foreach (Collider2D col in gameplayColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        gameplayColliders.Clear();
    }
}