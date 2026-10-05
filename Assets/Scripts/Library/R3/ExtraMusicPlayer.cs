using UnityEngine;
using System.Collections;

public class ExtraMusicPlayer : MonoBehaviour
{
    public AudioSource extraMusicSource;
    public AudioClip extraMusic;
    public float startDelay = 2f; // 기존 bgm 시작 타이밍(2초)과 맞추기 위한 지연

    void Start()
    {
        StartCoroutine(PlayExtraMusic());
    }

    IEnumerator PlayExtraMusic()
    {
        yield return new WaitForSeconds(startDelay);

        if (extraMusicSource != null && extraMusic != null)
        {
            extraMusicSource.clip = extraMusic;
            extraMusicSource.loop = true;
            extraMusicSource.Play();
        }
    }
}