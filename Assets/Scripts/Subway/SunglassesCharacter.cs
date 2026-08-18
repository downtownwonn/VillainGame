using UnityEngine;

public class SunglassesCharacter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite sunglassesSprite;
    public Sprite normalSprite;

    private bool sunglassesTaken = false;

    public void RemoveSunglasses()
    {
        if (sunglassesTaken)
            return;

        sunglassesTaken = true;

        spriteRenderer.sprite = normalSprite;

        Debug.Log("선글라스를 벗었습니다.");
    }
}