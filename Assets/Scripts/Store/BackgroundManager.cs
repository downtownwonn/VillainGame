using System.Collections;
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


    [Header("Screen Objects")]
    public GameObject leftObjects;
    public GameObject rightObjects;
    public GameObject warehouseObjects;


    [Header("Warehouse Lock")]
    public GameObject warehouseLock;


    private bool warehouseUnlocked = false;


    private void Start()
    {
        // 처음에는 왼쪽 화면
        backgroundImage.sprite = leftBackground;


        // 왼쪽 화살표는 숨김
        leftButton.SetActive(false);


        // 오른쪽 화살표는 표시
        rightButton.SetActive(true);


        // 왼쪽 화면 오브젝트 표시
        leftObjects.SetActive(true);


        // 오른쪽 화면과 창고 내부는 숨김
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
    }


    // 왼쪽 화면으로 이동
    public void MoveLeft()
    {
        backgroundImage.sprite = leftBackground;


        leftButton.SetActive(false);
        rightButton.SetActive(true);


        leftObjects.SetActive(true);
        rightObjects.SetActive(false);
    }


    // 자물쇠 해제
    public void UnlockWarehouse()
    {
        warehouseUnlocked = true;

        StartCoroutine(FadeOutLock());
    }


    // 자물쇠 서서히 사라지기
    private IEnumerator FadeOutLock()
    {
        Image lockImage = warehouseLock.GetComponent<Image>();

        Color color = lockImage.color;

        float fadeTime = 1f;
        float currentTime = 0f;


        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;

            color.a = Mathf.Lerp(
                1f,
                0f,
                currentTime / fadeTime
            );

            lockImage.color = color;

            yield return null;
        }


        color.a = 0f;
        lockImage.color = color;

        warehouseLock.SetActive(false);
    }


    // 창고문 클릭
    public void OpenWarehouse()
    {
        if (!warehouseUnlocked)
        {
            return;
        }


        backgroundImage.sprite = warehouseBackground;


        rightObjects.SetActive(false);


        warehouseObjects.SetActive(true);
    }
}