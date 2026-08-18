using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // Event Trigger 사용을 위해 필수!
using TMPro;

public class RemoteController : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI tempText;     // 온도 텍스트
    [SerializeField] private TextMeshProUGUI fanSpeedText; // 작대기 텍스트
    [SerializeField] private GameObject displayScreen;     // 디스플레이 화면 패널

    [Header("사운드 설정")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip powerSound;     // 전원 삐- 소리
    [SerializeField] private AudioClip buttonSound;    // 버튼 삑- 소리
    [SerializeField] private AudioClip breakSound;     // 폭주/고장 소리

    [Header("온도 설정")]
    [SerializeField] private int currentTemp = 24;
    [SerializeField] private int minTemp = 18;
    [SerializeField] private int maxTemp = 30;

    [Header("재미있는 연출 및 씬 설정")]
    [SerializeField] private string nextSceneName = "StoreR2Clear"; // 이동할 다음 씬 이름
    [SerializeField] private int targetFanPower = 20;               // 목표 작대기 개수

    [Header("꾹 누르기 속도 조절")]
    [SerializeField] private float initialDelay = 0.3f;  // 클릭 후 꾹 누르기로 인정되기까지 대기시간 (0.3초)
    [SerializeField] private float repeatRate = 0.1f;    // 꾹 누르는 동안 작대기 변하는 속도 (0.1초마다 1개)

    [Header("작대기 크기 연출 설정")]
    [SerializeField] private int sizeIncreaseStep = 15;  // 뒤로 갈수록 작대기가 커지는 비율 (%)

    private bool isPowerOn = false;
    private int currentFanPower = 3; // 기본 시작 작대기 3개

    // 홀드 상태 관련 변수
    private bool isFanUpHolding = false;
    private bool isFanDownHolding = false;
    private float holdTimer = 0f;
    private bool isRepeating = false; // 0.3초 대기시간이 지났는지 여부

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (!isPowerOn) return;

        // [바람 세기 ▲] 꾹 누르는 중
        if (isFanUpHolding)
        {
            holdTimer += Time.deltaTime;

            if (!isRepeating)
            {
                // 1. 처음 누르고 initialDelay(0.3초) 동안 대기
                if (holdTimer >= initialDelay)
                {
                    isRepeating = true;
                    holdTimer = 0f;
                }
            }
            else
            {
                // 2. 0.3초 지난 후부터 repeatRate(0.1초)마다 계속 증가
                if (holdTimer >= repeatRate)
                {
                    holdTimer = 0f;
                    AddFanPower();
                }
            }
        }

        // [바람 세기 ▼] 꾹 누르는 중
        if (isFanDownHolding)
        {
            holdTimer += Time.deltaTime;

            if (!isRepeating)
            {
                if (holdTimer >= initialDelay)
                {
                    isRepeating = true;
                    holdTimer = 0f;
                }
            }
            else
            {
                if (holdTimer >= repeatRate)
                {
                    holdTimer = 0f;
                    RemoveFanPower();
                }
            }
        }
    }

    // --- 전원 및 온도 버튼 ---

    public void TogglePower()
    {
        isPowerOn = !isPowerOn;
        PlaySound(powerSound);
        UpdateUI();
    }

    public void TempUp()
    {
        if (!isPowerOn) return;
        if (currentTemp < maxTemp)
        {
            currentTemp++;
            PlaySound(buttonSound);
            UpdateUI();
        }
    }

    public void TempDown()
    {
        if (!isPowerOn) return;
        if (currentTemp > minTemp)
        {
            currentTemp--;
            PlaySound(buttonSound);
            UpdateUI();
        }
    }

    // --- 바람 세기 이벤트 (Event Trigger 연결용) ---

    public void OnPointerDownFanUp()
    {
        if (!isPowerOn) return;

        ResetHoldState();
        isFanUpHolding = true;

        // 누른 순간 1개 즉시 증가!
        AddFanPower();
    }

    public void OnPointerUpFanUp()
    {
        ResetHoldState();
    }

    public void OnPointerDownFanDown()
    {
        if (!isPowerOn) return;

        ResetHoldState();
        isFanDownHolding = true;

        // 누른 순간 1개 즉시 감소!
        RemoveFanPower();
    }

    public void OnPointerUpFanDown()
    {
        ResetHoldState();
    }

    // 홀드 상태 초기화
    private void ResetHoldState()
    {
        isFanUpHolding = false;
        isFanDownHolding = false;
        isRepeating = false;
        holdTimer = 0f;
    }

    // 작대기 추가 처리
    private void AddFanPower()
    {
        currentFanPower++;
        PlaySound(buttonSound);
        UpdateUI();

        if (currentFanPower >= targetFanPower)
        {
            ResetHoldState();
            PlaySound(breakSound);
            Invoke(nameof(LoadNextScene), 0.3f);
        }
    }

    // 작대기 줄이기 처리
    private void RemoveFanPower()
    {
        if (currentFanPower > 1) // 최소 1개 유지
        {
            currentFanPower--;
            PlaySound(buttonSound);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (displayScreen != null)
        {
            displayScreen.SetActive(isPowerOn);
        }

        if (!isPowerOn) return;

        if (tempText != null)
        {
            tempText.text = $"{currentTemp}";
        }

        if (fanSpeedText != null)
        {
            string bars = "";

            // 작대기가 뒤로 갈수록 폰트 크기가 점진적으로 커지도록 처리
            for (int i = 0; i < currentFanPower; i++)
            {
                int fontSizePercent = 100 + (i * sizeIncreaseStep);
                bars += $"<size={fontSizePercent}%>|</size> ";
            }

            fanSpeedText.text = bars;
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}