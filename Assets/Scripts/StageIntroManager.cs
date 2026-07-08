using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIntroManager : MonoBehaviour
{
    public GameObject stagePanel;

    private bool canClick = false;

    void Start()
    {
        StartCoroutine(ShowStagePanel());
    }

    IEnumerator ShowStagePanel()
    {
        stagePanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        stagePanel.SetActive(false);

        canClick = true;
        Debug.Log("이제 클릭 가능");
    }

    void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("CinemaR1Play");
        }
    }
}