using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    public GameObject sitPlayer;
    public GameObject bendPlayer;
    public GameObject Canvas;

    void Start()
    {
        bendPlayer.SetActive(false); // ½ÃÀÛÇÏ¸é ¼÷ÀÎ »ç¶÷ ¼û±è
        Canvas.SetActive(true);
    }

    void OnMouseDown()
    {
        sitPlayer.SetActive(false);
        bendPlayer.SetActive(true);
    }
}