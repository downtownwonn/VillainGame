using UnityEngine;
using UnityEngine.EventSystems;

public class BubbleCharacter : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string dialogue;

    [Tooltip("If empty, the existing dialogue is used. Multiple lines cycle on each click.")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;
    private int nextDialogueLineIndex;

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

    public string GetNextDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            return dialogue;
        }

        string line = dialogueLines[nextDialogueLineIndex % dialogueLines.Length];
        nextDialogueLineIndex = (nextDialogueLineIndex + 1) % dialogueLines.Length;
        return string.IsNullOrWhiteSpace(line) ? dialogue : line;
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
        // UI 위를 클릭한 경우 캐릭터 클릭 무시
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (BubbleManager.Instance == null)
        {
            return;
        }

        BubbleManager.Instance.ShowBubble(this);
    }
}