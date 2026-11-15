using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Shows both volume sliders together and hides them after an outside click.</summary>
public sealed class AudioControlGroup : MonoBehaviour
{
    private Slider[] sliders = new Slider[0];
    private RectTransform[] icons = new RectTransform[0];
    private RectTransform homeButton;

    public void Configure(Transform controlRoot, RectTransform home, Slider bgm, Slider sfx)
    {
        homeButton = home;
        sliders = new[] { bgm, sfx };
        icons = new[]
        {
            FindIcon(controlRoot, "MusicIcon"),
            FindIcon(controlRoot, "SoundIcon")
        };
    }

    public void ToggleControls()
    {
        bool show = true;
        foreach (Slider slider in sliders)
        {
            if (slider != null && slider.gameObject.activeSelf)
            {
                show = false;
                break;
            }
        }

        SetControlsVisible(show);
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            HideIfOutside(Mouse.current.position.ReadValue());

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            HideIfOutside(Touchscreen.current.primaryTouch.position.ReadValue());
    }

    private void HideIfOutside(Vector2 screenPosition)
    {
        if (Contains(homeButton, screenPosition)) return;
        foreach (RectTransform icon in icons)
            if (Contains(icon, screenPosition)) return;
        foreach (Slider slider in sliders)
            if (slider != null && slider.gameObject.activeSelf && Contains(slider.transform as RectTransform, screenPosition)) return;

        SetControlsVisible(false);
    }

    private void SetControlsVisible(bool visible)
    {
        foreach (Slider slider in sliders)
            if (slider != null) slider.gameObject.SetActive(visible);
    }

    private static RectTransform FindIcon(Transform root, string iconName)
    {
        Transform icon = root != null ? root.Find(iconName) : null;
        return icon as RectTransform;
    }

    private static bool Contains(RectTransform rect, Vector2 screenPosition)
    {
        return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, null);
    }
}
