using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background")]
    public Image backgroundImage;

    public Sprite leftBackground;
    public Sprite rightBackground;
    public Sprite warehouseBackground;

    [Header("Buttons")]
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("Objects")]
    public GameObject leftObjects;
    public GameObject rightObjects;
    public GameObject warehouseObjects;

    // 창고가 열렸는지 여부
    private bool warehouseUnlocked = false;

    void Start()
    {
        // 시작 화면
        backgroundImage.sprite = leftBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(true);

        leftObjects.SetActive(true);
        rightObjects.SetActive(false);
        warehouseObjects.SetActive(false);
    }

    // 오른쪽 화면
    public void MoveRight()
    {
        backgroundImage.sprite = rightBackground;

        leftButton.SetActive(true);
        rightButton.SetActive(false);

        leftObjects.SetActive(false);
        rightObjects.SetActive(true);
        warehouseObjects.SetActive(false);
    }

    // 왼쪽 화면
    public void MoveLeft()
    {
        backgroundImage.sprite = leftBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(true);

        leftObjects.SetActive(true);
        rightObjects.SetActive(false);
        warehouseObjects.SetActive(false);
    }

    // 열쇠를 사용하면 호출
    public void UnlockWarehouse()
    {
        warehouseUnlocked = true;
    }

    // 창고문 클릭
    public void OpenWarehouse()
    {
        if (!warehouseUnlocked)
            return;

        backgroundImage.sprite = warehouseBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(false);

        leftObjects.SetActive(false);
        rightObjects.SetActive(false);
        warehouseObjects.SetActive(true);
    }
    
    // 창고에서 오른쪽 화면으로 돌아가기
    public void BackToRightRoom()
    {
        backgroundImage.sprite = rightBackground;

        leftButton.SetActive(true);
        rightButton.SetActive(false);

        warehouseObjects.SetActive(false);
        rightObjects.SetActive(true);
    }
}