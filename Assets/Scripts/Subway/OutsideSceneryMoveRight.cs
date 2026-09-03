using UnityEngine;

public class OutsideSceneryMove : MonoBehaviour
{
    public float speed = 0.2f;
    public float resetX = -10f;
    public float startX = 10f;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= resetX)
        {
            transform.position = new Vector3(
                startX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}