using UnityEngine;

public class BackgroundManager2 : MonoBehaviour
{
    [Header("Background")]
    public SpriteRenderer backgroundImage;

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

    [Header("Sound Effects")]



    [Tooltip("문이 열릴 때 소리")]
    public AudioClip openSound;



    void Start()
    {
        // 시작 화면 (왼쪽 방)
        backgroundImage.sprite = leftBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(true);

        leftObjects.SetActive(true);
        rightObjects.SetActive(false);
        warehouseObjects.SetActive(false);
    }

    // 오른쪽 화면으로 이동
    public void MoveRight()
    {
        backgroundImage.sprite = rightBackground;

        leftButton.SetActive(true);
        rightButton.SetActive(false);

        leftObjects.SetActive(false);
        rightObjects.SetActive(true);
        warehouseObjects.SetActive(false);
    }

    // 왼쪽 화면으로 이동 (오른쪽 방 -> 왼쪽 방)
    public void MoveLeft()
    {
        backgroundImage.sprite = leftBackground;

        leftButton.SetActive(false);
        rightButton.SetActive(true);

        leftObjects.SetActive(true);
        rightObjects.SetActive(false);
        warehouseObjects.SetActive(false);
    }

    // ★ 왼쪽 버튼을 눌렀을 때 현재 위치에 따라 다르게 동작하도록 처리하는 함수
    public void OnClickLeftButton()
    {
        // 현재 창고 안에 있다면 -> 오른쪽 방으로 돌아감
        if (warehouseObjects.activeSelf)
        {
            BackToRightRoom();
        }
        else
        {
            // 그 외(오른쪽 방에 있다면) -> 왼쪽 방으로 이동
            MoveLeft();
        }
    }

    // 창고문 클릭
    public void OpenWarehouse()
    {
       
  
        AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);

        backgroundImage.sprite = warehouseBackground;

        leftButton.SetActive(true);
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