using UnityEngine;

public class SafePasswordLock : MonoBehaviour
{
    [Header("비밀번호 설정")]
    [SerializeField] private string correctPassword = "365"; // '*' 제외한 숫자만 입력 (버튼 눌러서 검사하므로)
    private string currentInput = ""; // 현재 입력된 문자열

    [Header("열릴 때 활성화할 오브젝트")]
    [SerializeField] private GameObject openBackground; // 열린 금고 배경
    [SerializeField] private GameObject internalItems;   // 금고 내부 물체들

    [Header("사운드 설정")]
    [SerializeField] private AudioSource audioSource; // 소리를 재생할 AudioSource
    [SerializeField] private AudioClip successSound; // 철컥 소리 (성공)
    [SerializeField] private AudioClip failSound;    // 삐빅 소리 (실패)

    /// <summary>
    /// 키패드 버튼을 누를 때 호출되는 함수
    /// </summary>
    public void PressButton(string inputChar)
    {
        // '*' 버튼을 누르면 입력된 비밀번호 검사 실행
        if (inputChar == "*")
        {
            CheckPassword();
            return;
        }

        // 비밀번호 입력 추가 (숫자 및 기타 버튼)
        currentInput += inputChar;
        Debug.Log($"현재 입력된 비밀번호: {currentInput}");
    }

    /// <summary>
    /// 비밀번호 정답 여부 체크
    /// </summary>
    private void CheckPassword()
    {
        // 정답과 일치하는 경우
        if (currentInput == correctPassword)
        {
            OpenSafe();
        }
        else
        {
            // 틀렸을 경우: 실패 사운드 재생 후 입력 초기화
            Debug.Log("비밀번호 불일치! 입력이 초기화됩니다.");
            PlaySound(failSound);
            ClearInput();
        }
    }

    /// <summary>
    /// 금고가 열릴 때 실행
    /// </summary>
    private void OpenSafe()
    {
        Debug.Log("금고가 열렸습니다!");

        // 성공 사운드 재생
        PlaySound(successSound);

        // 열린 배경 및 내부 물체 활성화
        if (openBackground != null) openBackground.SetActive(true);
        if (internalItems != null) internalItems.SetActive(true);

        // 입력값 초기화
        ClearInput();
    }

    /// <summary>
    /// 오디오 재생 전용 함수
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 입력 초기화
    /// </summary>
    public void ClearInput()
    {
        currentInput = "";
    }
}