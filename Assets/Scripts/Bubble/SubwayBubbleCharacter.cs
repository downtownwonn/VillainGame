using UnityEngine;
using UnityEngine.EventSystems;

public class SubwayBubbleCharacter : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string dialogue;

    [Header("Optional Dialogue Cycle")]
    [Tooltip("If entries are set, the bubble advances through them each time it is shown.")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueCycle;
    private int nextDialogueIndex;

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

    public string GetDialogueForDisplay()
    {
        if (dialogueCycle == null || dialogueCycle.Length == 0)
        {
            return dialogue;
        }

        string nextDialogue = dialogueCycle[nextDialogueIndex];
        nextDialogueIndex = (nextDialogueIndex + 1) % dialogueCycle.Length;

        return string.IsNullOrEmpty(nextDialogue) ? dialogue : nextDialogue;
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
        // 장구부원이 상모를 받고 춤추는 동안에는 기존 캐릭터의 넓은 콜라이더가
        // 빈 공간 클릭까지 잡을 수 있으므로 말풍선을 띄우지 않는다.
        JangguCharacter jangguCharacter = GetComponentInParent<JangguCharacter>();
        if (jangguCharacter != null && jangguCharacter.IsDancing)
        {
            return;
        }

        // UI ���� Ŭ���� ��� ĳ���� Ŭ�� ����
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (SubwayBubbleManager.Instance == null)
        {
            return;
        }

        SubwayBubbleManager.Instance.ShowBubble(this);
    }
}
