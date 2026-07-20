using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // 게임 시작 관련 소리
    public AudioClip introSound;
    public AudioClip bgm;

    // 클릭 소리
    public AudioClip uiClickSound;
    public AudioClip objectClickSound;

    public AudioSource ambienceSource;
    public AudioSource kissSource;

    public AudioClip projectorSound;
    public AudioClip kissSound;


    void Start()
    {
        StartCoroutine(StartGameMusic());
    }


    // 인트로 → BGM 재생
    IEnumerator StartGameMusic()
    {
        // 라운드 시작 효과음
        sfxSource.PlayOneShot(introSound);

        yield return new WaitForSeconds(2f);


        // BGM 시작
        bgmSource.clip = bgm;
        bgmSource.loop = true;
        bgmSource.Play();


        // 영사기 소리 시작
        ambienceSource.clip = projectorSound;
        ambienceSource.loop = true;
        ambienceSource.Play();


        // 키스 소리 시작
        kissSource.clip = kissSound;
        kissSource.loop = true;
        kissSource.Play();
    }


    // UI 버튼 클릭음
    public void PlayUIClick()
    {
        sfxSource.PlayOneShot(uiClickSound);
    }


    // 게임 오브젝트 클릭음
    public void PlayObjectClick()
    {
        sfxSource.PlayOneShot(objectClickSound);
    }
}