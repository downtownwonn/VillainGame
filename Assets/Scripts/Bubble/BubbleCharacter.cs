using UnityEngine;

public class BubbleCharacter : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string dialogue;

    [Header("Bubble Type")]
    [SerializeField] private int bubbleType;

    [Header("Bubble Position Offset")]
    [SerializeField] private Vector2 bubbleOffset = Vector2.zero;

    [Header("Text Position Offset")]
    [SerializeField] private Vector2 textOffset = Vector2.zero;

    [Header("Sound")]
    [SerializeField] private AudioClip sound;

    public string Dialogue
    {
        get { return dialogue; }
    }

    public int BubbleType
    {
        get { return bubbleType; }
    }

    public Vector2 BubbleOffset
    {
        get { return bubbleOffset; }
    }

    public Vector2 TextOffset
    {
        get { return textOffset; }
    }

    public AudioClip Sound
    {
        get { return sound; }
    }

    private void OnMouseDown()
    {
        if (BubbleManager.Instance == null)
        {
            return;
        }

        BubbleManager.Instance.ShowBubble(this);
    }
}