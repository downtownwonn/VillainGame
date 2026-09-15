using UnityEngine;

public class VillainFootIdleMotion : MonoBehaviour
{
    [Header("Idle Foot Motion")]
    [Tooltip("무릎을 기준으로 흔들리는 최대 각도")]
    public float shakeAngle = 3f;

    [Tooltip("흔들리는 속도")]
    public float shakeSpeed = 2f;

    [Tooltip("흔들림 시작 위치를 랜덤하게 할지 여부")]
    public bool randomStart = true;

    private Quaternion originalRotation;
    private float randomOffset;

    private void Start()
    {
        // 현재 회전값 저장
        originalRotation = transform.localRotation;

        // 여러 빌런이 있다면 동시에 똑같이 움직이지 않도록 약간의 차이
        randomOffset = randomStart
            ? Random.Range(0f, Mathf.PI * 2f)
            : 0f;
    }

    private void Update()
    {
        float angle =
            Mathf.Sin(Time.time * shakeSpeed + randomOffset)
            * shakeAngle;

        transform.localRotation =
            originalRotation *
            Quaternion.Euler(0f, 0f, angle);
    }
}