using UnityEngine;

public class SangmoCharacter : MonoBehaviour
{
    public GameObject sangmoCharacter;
    public GameObject normalCharacter;

    public void ChangeToNormal()
    {
        sangmoCharacter.SetActive(false);
        normalCharacter.SetActive(true);
    }
}