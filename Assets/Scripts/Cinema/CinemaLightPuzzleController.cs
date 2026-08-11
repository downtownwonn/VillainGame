using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CinemaLightPuzzleController : MonoBehaviour
{
    [Header("Popup")]
    public GameObject boxPopup;

    [Header("Buttons")]
    public Button lightButton;

    [Header("Seat Lines")]
    public Transform lineA;
    public Transform lineB;
    public Transform lineC;
    public Transform lineD;
    public Transform lineE;

    [Header("Seat Colors")]
    public Color offColor = new Color(0.45f, 0.45f, 0.45f);
    public Color onColor = Color.yellow;

    [Header("Timing")]

    [Tooltip("팝업이 열린 후 게임 시작까지 기다리는 시간")]
    public float startDelay = 3f;

    [Tooltip("좌석 하나가 켜져있는 시간")]
    public float blinkTime = 0.18f;

    [Tooltip("성공 후 씬 이동까지 기다리는 시간")]
    public float clearDelay = 0.4f;

    [Tooltip("실패 후 팝업 닫기까지 기다리는 시간")]
    public float failDelay = 0.4f;

    [Header("Target Seat")]
    [Tooltip("A1=0, D12=59")]
    public int targetSeatIndex = 59;

    [Header("Mini Game Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip miniGameBGM;
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip lightOnClip;

    // ★추가
    [Header("Game BGM")]
    public AudioSource gameBGM;

    List<Image> seats = new List<Image>();

    int currentSeat = -1;

    bool gameRunning = false;
    bool finished = false;

    Coroutine gameRoutine;

    void Start()
    {
        CollectSeats();

        SetAllSeatColor(offColor);

        lightButton.onClick.AddListener(CheckAnswer);
    }

    void CollectSeats()
    {
        seats.Clear();

        AddLine(lineA);
        AddLine(lineB);
        AddLine(lineC);
        AddLine(lineD);
        AddLine(lineE);
    }

    void AddLine(Transform line)
    {
        foreach (Transform child in line)
        {
            Image img = child.GetComponent<Image>();

            if (img != null)
            {
                img.color = offColor;
                seats.Add(img);
            }
        }
    }

    void SetAllSeatColor(Color c)
    {
        foreach (Image img in seats)
        {
            img.color = c;
        }
    }

    //=========================================
    // 팝업 열기
    //=========================================

    public void OpenPuzzle()
    {

        Debug.Log("OpenPuzzle called");

        boxPopup.SetActive(true);

        SetAllSeatColor(offColor);

        currentSeat = -1;

        finished = false;
        gameRunning = false;

        if (gameRoutine != null)
            StopCoroutine(gameRoutine);

        gameRoutine = StartCoroutine(StartPuzzleRoutine());
    }

    IEnumerator StartPuzzleRoutine()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        if (finished)
            yield break;

        // ★추가
        if (gameBGM != null)
            gameBGM.Pause();

        gameRunning = true;

        if (bgmSource != null && miniGameBGM != null)
        {
            bgmSource.clip = miniGameBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        yield return StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < seats.Count; i++)
        {
            if (finished)
                yield break;

            if (currentSeat >= 0)
                seats[currentSeat].color = offColor;

            currentSeat = i;

            seats[currentSeat].color = onColor;

            yield return new WaitForSecondsRealtime(blinkTime);
        }

        // 끝까지 버튼을 누르지 않음
        if (!finished)
        {
            finished = true;

            StartCoroutine(FailRoutine());
        }
    }

    void CheckAnswer()
    {
        if (!gameRunning)
            return;

        if (finished)
            return;

        if (currentSeat == targetSeatIndex)
        {
            finished = true;

            StartCoroutine(ClearRoutine());
        }
        else
        {
            finished = true;

            StartCoroutine(FailRoutine());
        }
    }

    //=========================================
    // 성공
    //=========================================

    IEnumerator ClearRoutine()
    {
        gameRunning = false;

        if (currentSeat >= 0 && currentSeat < seats.Count)
            seats[currentSeat].color = offColor;

        // 미니게임 BGM 정지
        if (bgmSource != null)
            bgmSource.Stop();

        // 성공 효과음
        if (sfxSource != null && successClip != null)
        {
            sfxSource.PlayOneShot(successClip);
            yield return new WaitForSecondsRealtime(successClip.length);
        }

        // 조명 켜지는 효과음
        if (sfxSource != null && lightOnClip != null)
        {
            sfxSource.PlayOneShot(lightOnClip);
            yield return new WaitForSecondsRealtime(lightOnClip.length);
        }

        // 원래 게임 BGM 다시 재생
        if (gameBGM != null)
            gameBGM.UnPause();

        if (clearDelay > 0)
            yield return new WaitForSecondsRealtime(clearDelay);

        SceneManager.LoadScene("CinemaR3Clear");
    }


    //=========================================
    // 실패
    //=========================================

    IEnumerator FailRoutine()
    {
        gameRunning = false;

        if (currentSeat >= 0 && currentSeat < seats.Count)
            seats[currentSeat].color = offColor;

        // 미니게임 BGM 정지
        if (bgmSource != null)
            bgmSource.Stop();

        // 실패 효과음
        if (sfxSource != null && failClip != null)
        {
            sfxSource.PlayOneShot(failClip);
            yield return new WaitForSecondsRealtime(failClip.length);
        }

        if (failDelay > 0)
            yield return new WaitForSecondsRealtime(failDelay);

        ClosePuzzle();
    }


    //=========================================
    // 팝업 닫기
    //=========================================

    public void ClosePuzzle()
    {
        if (gameRoutine != null)
        {
            StopCoroutine(gameRoutine);
            gameRoutine = null;
        }

        StopAllCoroutines();

        // 미니게임 BGM 종료
        if (bgmSource != null)
            bgmSource.Stop();

        // 원래 게임 BGM 이어서 재생
        if (gameBGM != null)
            gameBGM.UnPause();

        ResetPuzzle();

        if (boxPopup != null)
            boxPopup.SetActive(false);
    }


    //=========================================
    // 초기화
    //=========================================

    void ResetPuzzle()
    {
        SetAllSeatColor(offColor);

        currentSeat = -1;

        finished = false;
        gameRunning = false;
    }


    //=========================================
    // 오브젝트가 비활성화될 때
    //=========================================

    void OnDisable()
    {
        if (bgmSource != null)
            bgmSource.Stop();

        if (gameBGM != null)
            gameBGM.UnPause();

        ResetPuzzle();
    }
}