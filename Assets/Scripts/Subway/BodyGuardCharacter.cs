using UnityEngine;

public class BodyguardCharacter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite normalSprite;
    public Sprite sunglassesSprite;
    public Sprite bodyguardSprite;

    public BodyguardGroupManager groupManager;

    private bool hasSunglasses = false;
    private bool isBodyguard = false;


    public void PutOnSunglasses()
    {
        if (hasSunglasses)
            return;

        hasSunglasses = true;

        spriteRenderer.sprite = sunglassesSprite;

        Debug.Log(gameObject.name + " 선글라스 착용 완료");

        if (groupManager != null)
        {
            groupManager.SunglassesCompleted(this);
        }
    }


    public void BecomeBodyguard()
    {
        if (!hasSunglasses || isBodyguard)
            return;

        isBodyguard = true;

        spriteRenderer.sprite = bodyguardSprite;

        transform.localScale = new Vector3(1.4f, 1.4f, 1f);

        Debug.Log(gameObject.name + " 경호원으로 변신!");
    }


    public bool HasSunglasses()
    {
        return hasSunglasses;
    }


    public bool IsBodyguard()
    {
        return isBodyguard;
    }
    private void OnMouseDown()
{
    if (!isBodyguard)
        return;

    if (groupManager != null)
    {
        groupManager.CollectBodyguards();
    }
}
}