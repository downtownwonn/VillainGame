using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Image backgroundImage;

    public Sprite leftBackground;
    public Sprite rightBackground;

    public GameObject leftButton;
    public GameObject rightButton;

    void Start()
    {
        // 처음에는 왼쪽 화면
        backgroundImage.sprite = leftBackground;

        // 왼쪽 화살표는 숨김
        leftButton.SetActive(false);

        // 오른쪽 화살표는 표시
        rightButton.SetActive(true);
    }

    public void MoveRight()
    {
        backgroundImage.sprite = rightBackground;

        rightButton.SetActive(false);
        leftButton.SetActive(true);
    }

    public void MoveLeft()
    {
        backgroundImage.sprite = leftBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(true);
    }
}