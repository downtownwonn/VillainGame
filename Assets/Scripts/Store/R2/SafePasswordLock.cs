using UnityEngine;
using TMPro; // TextMeshPro 사용 시 (일반 Text 사용 시 using UnityEngine.UI;)
using System.Collections;

public class SafePasswordLock : MonoBehaviour
{
    [Header("비밀번호 설정")]
    [SerializeField] private string correctPassword = "386"; 
    private string currentInput = ""; 
    private bool isUnlocked = false;

    [Header("UI 텍스트 설정")]
    [SerializeField] private TextMeshProUGUI displayText; 
    [SerializeField] private string defaultPrompt = "비밀번호를 입력하세요"; // 안내 문구

    [Header("UI 색상 설정")]
    [SerializeField] private Color normalColor = Color.white;  // 평소 / 숫자 입력 중 색상
    [SerializeField] private Color successColor = Color.green; // 성공 시 색상
    [SerializeField] private Color failColor = Color.red;     // 실패 시 색상

    [Header("금고 열림 지연 설정")]
    [SerializeField] private float openDelay = 1.5f; // 정답 맞추고 금고 문이 열리기까지의 대기 시간(초)

    [Header("열릴 때 활성화할 오브젝트")]
    [SerializeField] private GameObject openBackground; 
    [SerializeField] private GameObject internalItems;   

    [Header("사운드 설정")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip successSound; 
    [SerializeField] private AudioClip failSound;    

    private void Start()
    {
        UpdateDisplay(GetMaskedInput(), normalColor);
    }

    /// <summary>
    /// 키패드 버튼을 누를 때 호출되는 함수
    /// </summary>
    public void PressButton(string inputChar)
    {
        if (isUnlocked) return;

        if (inputChar == "*")
        {
            CheckPassword();
            return;
        }

        // 비밀번호 자릿수(correctPassword.Length)까지만 입력을 받도록 제한
        if (currentInput.Length < correctPassword.Length)
        {
            currentInput += inputChar;
        }

        UpdateDisplay(GetMaskedInput(), normalColor);
    }

    /// <summary>
    /// 항상 안내 문구와 띄어진 마스킹 문자열을 함께 반환 (예: "비밀번호를 입력하세요 3 _ _")
    /// </summary>
    private string GetMaskedInput()
    {
        int passwordLength = correctPassword.Length;
        System.Collections.Generic.List<string> chars = new System.Collections.Generic.List<string>();

        for (int i = 0; i < passwordLength; i++)
        {
            if (i < currentInput.Length)
            {
                chars.Add(currentInput[i].ToString()); // 입력된 글자 추가
            }
            else
            {
                chars.Add("_"); // 아직 입력되지 않은 자리는 '_' 추가
            }
        }

        string masked = string.Join(" ", chars);

        // 항상 안내 문구와 마스킹된 문자열을 조합해서 반환
        return defaultPrompt + " " + masked;
    }

    /// <summary>
    /// 비밀번호 정답 여부 체크
    /// </summary>
    private void CheckPassword()
    {
        if (currentInput == correctPassword)
        {
            StartCoroutine(OpenSafeRoutine());
        }
        else
        {
            PlaySound(failSound);
            StartCoroutine(ShowFailedMessage());
        }
    }

    /// <summary>
    /// 금고가 열릴 때 실행 (지연 시간 적용)
    /// </summary>
    private IEnumerator OpenSafeRoutine()
    {
        isUnlocked = true;

        UpdateDisplay("금고문이 열렸습니다!", successColor);
        PlaySound(successSound);

        yield return new WaitForSeconds(openDelay);

        if (openBackground != null) openBackground.SetActive(true);
        if (internalItems != null) internalItems.SetActive(true);

        currentInput = "";
    }

    /// <summary>
    /// 틀렸을 때 빨간색으로 메시지를 보여주고 1초 후 초기화
    /// </summary>
    private IEnumerator ShowFailedMessage()
    {
        UpdateDisplay("다시 시도해주세요", failColor);
        
        yield return new WaitForSeconds(1.0f); // 1초 대기

        ClearInput();
    }

    /// <summary>
    /// 오디오 재생
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 입력 및 텍스트 초기화 (기본 색상으로 복귀)
    /// </summary>
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay(GetMaskedInput(), normalColor);
    }

    /// <summary>
    /// UI 텍스트 및 색상 업데이트
    /// </summary>
    private void UpdateDisplay(string text, Color textColor)
    {
        if (displayText != null)
        {
            displayText.text = text;
            displayText.color = textColor;
        }
    }
}