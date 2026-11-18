using UnityEngine;

[CreateAssetMenu(menuName = "UI/Menu Audio Control Icons")]
public sealed class MenuAudioControlIconSet : ScriptableObject
{
    public Sprite bgmIcon;
    public Sprite sfxIcon;
    public Sprite homeIcon;
    public Sprite volumeKnob;
    public AudioClip controlClickClip;
}
