using UnityEngine;

public sealed class GameplayModalPanel : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetAsLastSibling();
        HomeButtonBootstrap.SetModalPopupOpen(gameObject, true);
    }

    private void OnDisable() => HomeButtonBootstrap.SetModalPopupOpen(gameObject, false);
}
