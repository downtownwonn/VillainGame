using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Shared pixel-style colors, buttons, and line-and-knob volume controls.</summary>
public static class MenuUITheme
{
    public static readonly Color Panel = new Color32(13, 15, 15, 246);
    public static readonly Color Accent = new Color32(240, 240, 240, 255);
    public static readonly Color Track = new Color32(22, 22, 22, 245);

    private static Sprite circleSprite;
    private static Sprite speakerSprite;
    private static Sprite musicSprite;
    private static Sprite knobSprite;

    public static void StyleButton(Button button, TMP_FontAsset menuFont)
    {
        if (button == null)
        {
            return;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(220, 220, 220, 255);
        colors.pressedColor = new Color32(175, 175, 175, 255);
        colors.selectedColor = new Color32(235, 235, 235, 255);
        colors.disabledColor = new Color32(38, 40, 39, 150);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;

        Graphic graphic = button.targetGraphic;
        if (graphic == null)
        {
            graphic = button.GetComponent<Image>();
            button.targetGraphic = graphic;
        }

        Image image = graphic as Image;
        if (image != null)
        {
            image.color = Color.white;
            image.type = Image.Type.Simple;
        }

        Outline outline = button.GetComponent<Outline>();
        if (outline != null)
        {
            Object.Destroy(outline);
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            if (menuFont != null)
            {
                label.font = menuFont;
            }
            label.color = Color.white;
            label.fontStyle = FontStyles.Bold;
        }
    }

    public static Image CreateControlPanel(Transform parent, string name, Vector2 position, Vector2 size)
    {
        Image panel = CreateImage(name, parent, Color.clear);
        panel.raycastTarget = false;
        RectTransform rect = panel.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Outline outline = panel.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(Accent.r, Accent.g, Accent.b, 0.7f);
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = false;
        panel.transform.SetAsLastSibling();
        return panel;
    }

    public static Slider CreateVolumeControl(
        Transform parent,
        string label,
        Vector2 labelPosition,
        Vector2 sliderPosition,
        float sliderWidth,
        float initialValue,
        TMP_FontAsset menuFont,
        UnityAction<float> onValueChanged)
    {
        GameObject labelObject = new GameObject(label + "Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(parent, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = labelPosition;
        labelRect.sizeDelta = new Vector2(110f, 42f);
        TMP_Text text = labelObject.GetComponent<TMP_Text>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 25f;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.raycastTarget = false;
        if (menuFont != null)
        {
            text.font = menuFont;
        }

        GameObject sliderObject = new GameObject(label + "VolumeSlider", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Slider));
        sliderObject.transform.SetParent(parent, false);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = sliderPosition;
        sliderRect.sizeDelta = new Vector2(sliderWidth, 36f);
        Image inputGraphic = sliderObject.GetComponent<Image>();
        inputGraphic.color = Color.clear;
        inputGraphic.raycastTarget = true;

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.direction = Slider.Direction.LeftToRight;

        Image track = CreateImage("Track", sliderObject.transform, Track);
        RectTransform trackRect = track.rectTransform;
        trackRect.anchorMin = new Vector2(0f, 0.5f);
        trackRect.anchorMax = new Vector2(1f, 0.5f);
        trackRect.offsetMin = new Vector2(12f, 0f);
        trackRect.offsetMax = new Vector2(-12f, 0f);
        trackRect.sizeDelta = new Vector2(0f, 4f);
        track.raycastTarget = false;

        Image handle = CreateImage("RoundHandle", sliderObject.transform, Accent);
        handle.sprite = CircleSprite;
        handle.preserveAspect = true;
        RectTransform handleRect = handle.rectTransform;
        handleRect.anchorMin = handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.sizeDelta = new Vector2(22f, 22f);
        handle.raycastTarget = true;

        slider.fillRect = null;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.SetValueWithoutNotify(Mathf.Clamp01(initialValue));
        if (onValueChanged != null)
        {
            slider.onValueChanged.AddListener(onValueChanged);
        }
        AddSliderClickSound(slider);
        return slider;
    }

    public static Slider CreateVerticalAudioControl(
        Transform parent,
        bool musicIcon,
        Vector2 iconPosition,
        Vector2 sliderPosition,
        float sliderHeight,
        float initialValue,
        UnityAction<float> onValueChanged,
        Sprite customIconSprite = null,
        Sprite[] customIconParts = null,
        Sprite customKnobSprite = null)
    {
        bool hasCompositeIcon = customIconParts != null && customIconParts.Length > 0;
        GameObject iconObject = new GameObject(musicIcon ? "MusicIcon" : "SoundIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObject.transform.SetParent(parent, false);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = iconPosition;
        iconRect.sizeDelta = new Vector2(44f, 44f);
        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = CircleSprite;
        icon.color = new Color32(12, 12, 12, 255);
        icon.preserveAspect = true;
        icon.raycastTarget = true;
        Outline iconOutline = iconObject.AddComponent<Outline>();
        iconOutline.effectColor = new Color32(185, 185, 185, 230);
        iconOutline.effectDistance = new Vector2(1f, -1f);
        if (customIconSprite != null)
        {
            icon.sprite = customIconSprite;
            icon.color = Color.white;
            iconOutline.enabled = false;
        }
        else if (hasCompositeIcon)
        {
            icon.sprite = null;
            icon.color = Color.clear;
            iconOutline.enabled = false;
        }
        Button iconButton = iconObject.AddComponent<Button>();
        iconButton.targetGraphic = icon;
        iconButton.transition = Selectable.Transition.None;

        GameObject glyphObject = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        glyphObject.transform.SetParent(iconObject.transform, false);
        RectTransform glyphRect = glyphObject.GetComponent<RectTransform>();
        glyphRect.anchorMin = Vector2.zero;
        glyphRect.anchorMax = Vector2.one;
        glyphRect.offsetMin = new Vector2(8f, 8f);
        glyphRect.offsetMax = new Vector2(-8f, -8f);
        Image glyph = glyphObject.GetComponent<Image>();
        glyph.sprite = customIconSprite != null || hasCompositeIcon ? null : CreateAudioIcon(musicIcon);
        glyph.color = Color.white;
        glyph.preserveAspect = true;
        glyph.raycastTarget = false;
        glyphObject.SetActive(customIconSprite == null && !hasCompositeIcon);
        if (hasCompositeIcon)
        {
            CreateCompositeAudioIcon(iconObject.transform, iconRect, customIconParts);
        }

        CanvasGroup iconCanvasGroup = iconObject.AddComponent<CanvasGroup>();
        bool initiallyMuted = musicIcon ? MenuAudioManager.IsBgmMuted() : MenuAudioManager.IsButtonMuted();
        iconCanvasGroup.alpha = initiallyMuted ? 0.5f : 1f;
        iconButton.onClick.AddListener(() =>
        {
            bool muted = musicIcon ? MenuAudioManager.ToggleBgmMute() : MenuAudioManager.ToggleButtonMute();
            iconCanvasGroup.alpha = muted ? 0.5f : 1f;
        });

        GameObject sliderObject = new GameObject(musicIcon ? "BGMVolumeSlider" : "SFXVolumeSlider", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Slider));
        sliderObject.transform.SetParent(parent, false);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = sliderPosition;
        sliderRect.sizeDelta = new Vector2(36f, sliderHeight);
        Image inputGraphic = sliderObject.GetComponent<Image>();
        inputGraphic.color = Color.clear;
        inputGraphic.raycastTarget = true;

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.direction = Slider.Direction.BottomToTop;

        Image track = CreateImage("Track", sliderObject.transform, Track);
        RectTransform trackRect = track.rectTransform;
        trackRect.anchorMin = new Vector2(0.5f, 0f);
        trackRect.anchorMax = new Vector2(0.5f, 1f);
        trackRect.offsetMin = new Vector2(0f, 12f);
        trackRect.offsetMax = new Vector2(0f, -12f);
        trackRect.sizeDelta = new Vector2(4f, 0f);
        track.raycastTarget = false;
        Outline trackOutline = track.gameObject.AddComponent<Outline>();
        trackOutline.effectColor = new Color32(155, 155, 155, 220);
        trackOutline.effectDistance = new Vector2(1f, -1f);

        Image handle = CreateImage("RoundHandle", sliderObject.transform, Color.white);
        RectTransform handleRect = handle.rectTransform;
        handleRect.anchorMin = handleRect.anchorMax = new Vector2(0.5f, 1f);
        handleRect.sizeDelta = new Vector2(26f, 26f);
        handle.raycastTarget = true;
        handle.sprite = customKnobSprite != null ? customKnobSprite : KnobSprite;
        handle.color = Color.white;
        handle.type = Image.Type.Simple;
        handle.preserveAspect = true;
        Outline handleOutline = handle.gameObject.AddComponent<Outline>();
        handleOutline.effectColor = new Color32(24, 24, 24, 255);
        handleOutline.effectDistance = new Vector2(1f, -1f);
        handleOutline.enabled = customKnobSprite == null;

        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.SetValueWithoutNotify(Mathf.Clamp01(initialValue));
        if (onValueChanged != null)
        {
            slider.onValueChanged.AddListener(onValueChanged);
        }
        AddSliderClickSound(slider);
        return slider;
    }

    private static void AddSliderClickSound(Slider slider)
    {
        EventTrigger trigger = slider.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener(_ => MenuAudioManager.PlayButtonClick());
        trigger.triggers.Add(pointerDown);

        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener(_ => MenuAudioManager.PlaySliderRelease());
        trigger.triggers.Add(pointerUp);
    }

    private static void CreateCompositeAudioIcon(Transform parent, RectTransform iconRect, Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0 || sprites[0] == null || sprites[0].texture == null)
        {
            return;
        }

        Texture texture = sprites[0].texture;
        float scale = Mathf.Min(iconRect.sizeDelta.x / texture.width, iconRect.sizeDelta.y / texture.height);
        Vector2 textureCenter = new Vector2(texture.width * 0.5f, texture.height * 0.5f);
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];
            if (sprite == null || sprite.texture != texture)
            {
                continue;
            }

            Rect sourceRect = sprite.rect;
            GameObject partObject = new GameObject($"IconPart_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            partObject.transform.SetParent(parent, false);
            RectTransform partRect = partObject.GetComponent<RectTransform>();
            partRect.anchorMin = partRect.anchorMax = new Vector2(0.5f, 0.5f);
            partRect.sizeDelta = sourceRect.size * scale;
            partRect.anchoredPosition = (sourceRect.center - textureCenter) * scale;

            Image partImage = partObject.GetComponent<Image>();
            partImage.sprite = sprite;
            partImage.color = Color.white;
            partImage.preserveAspect = true;
            partImage.raycastTarget = false;
        }
    }

    public static void StyleExistingSlider(Slider slider)
    {
        if (slider == null)
        {
            return;
        }

        if (slider.fillRect != null)
        {
            slider.fillRect.gameObject.SetActive(false);
            slider.fillRect = null;
        }

        Image inputGraphic = slider.GetComponent<Image>();
        if (inputGraphic != null)
        {
            inputGraphic.color = Color.clear;
            inputGraphic.raycastTarget = true;
        }

        Transform trackTransform = slider.transform.Find("MenuTrack");
        Image track;
        if (trackTransform == null)
        {
            track = CreateImage("MenuTrack", slider.transform, Track);
            trackTransform = track.transform;
        }
        else
        {
            track = trackTransform.GetComponent<Image>();
        }

        RectTransform trackRect = track.rectTransform;
        trackRect.anchorMin = new Vector2(0f, 0.5f);
        trackRect.anchorMax = new Vector2(1f, 0.5f);
        trackRect.offsetMin = new Vector2(12f, 0f);
        trackRect.offsetMax = new Vector2(-12f, 0f);
        trackRect.sizeDelta = new Vector2(0f, 4f);
        track.raycastTarget = false;
        track.transform.SetAsFirstSibling();

        RectTransform handleRect = slider.handleRect;
        if (handleRect == null)
        {
            GameObject handleObject = new GameObject("RoundHandle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handleObject.transform.SetParent(slider.transform, false);
            handleRect = handleObject.GetComponent<RectTransform>();
            slider.handleRect = handleRect;
        }

        Image handle = handleRect.GetComponent<Image>();
        if (handle == null)
        {
            handle = handleRect.gameObject.AddComponent<Image>();
        }
        handle.sprite = KnobSprite;
        handle.type = Image.Type.Simple;
        handle.preserveAspect = true;
        handle.color = Accent;
        handle.raycastTarget = true;
        handleRect.anchorMin = handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.sizeDelta = new Vector2(22f, 22f);
        handleRect.SetAsLastSibling();

        slider.fillRect = null;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Sprite CircleSprite
    {
        get
        {
            if (circleSprite != null)
            {
                return circleSprite;
            }

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "RuntimeMenuSliderCircle";
            texture.hideFlags = HideFlags.HideAndDontSave;
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = 1f - Mathf.SmoothStep(size * 0.45f, size * 0.48f, distance);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(false, false);
            circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
            circleSprite.name = "RuntimeMenuSliderCircle";
            circleSprite.hideFlags = HideFlags.HideAndDontSave;
            return circleSprite;
        }
    }

    private static Sprite KnobSprite
    {
        get
        {
            if (knobSprite != null)
            {
                return knobSprite;
            }

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false, true)
            {
                name = "RuntimeBlackVolumeKnob",
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float radius = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    float alpha = 1f - Mathf.SmoothStep(0.46f, 0.49f, radius);
                    float whiteRing = 1f - Mathf.SmoothStep(0.32f, 0.38f, radius);
                    Color color = Color.Lerp(Color.black, Color.white, whiteRing);
                    color.a = alpha;
                    pixels[y * size + x] = color;
                }

            texture.SetPixels(pixels);
            texture.Apply(false, false);
            knobSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
            knobSprite.name = "RuntimeBlackVolumeKnob";
            knobSprite.hideFlags = HideFlags.HideAndDontSave;
            return knobSprite;
        }
    }

    private static Sprite CreateAudioIcon(bool music)
    {
        if (music && musicSprite != null) return musicSprite;
        if (!music && speakerSprite != null) return speakerSprite;

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false, true)
        {
            name = music ? "RuntimeMusicIcon" : "RuntimeSpeakerIcon",
            hideFlags = HideFlags.HideAndDontSave
        };
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

        if (music)
        {
            FillRect(pixels, size, 37, 12, 43, 49);
            FillRect(pixels, size, 40, 11, 55, 17);
            FillRect(pixels, size, 50, 12, 56, 24);
            FillEllipse(pixels, size, 15, 43, 14, 9);
            FillEllipse(pixels, size, 34, 38, 13, 9);
        }
        else
        {
            FillRect(pixels, size, 8, 25, 21, 39);
            FillTriangle(pixels, size, 20, 25, 43, 10, 43, 54);
            DrawArc(pixels, size, 32, 32, 18, 23);
            DrawArc(pixels, size, 32, 32, 25, 31);
        }

        texture.SetPixels(pixels);
        texture.Apply(false, false);
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        if (music) musicSprite = sprite; else speakerSprite = sprite;
        return sprite;
    }

    private static void FillRect(Color[] pixels, int size, int xMin, int yMin, int xMax, int yMax)
    {
        for (int y = yMin; y <= yMax; y++)
            for (int x = xMin; x <= xMax; x++)
                pixels[y * size + x] = Color.white;
    }

    private static void FillEllipse(Color[] pixels, int size, int centerX, int centerY, int radiusX, int radiusY)
    {
        for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
            for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                float dx = (x - centerX) / (float)radiusX;
                float dy = (y - centerY) / (float)radiusY;
                if (dx * dx + dy * dy <= 1f) pixels[y * size + x] = Color.white;
            }
    }

    private static void FillTriangle(Color[] pixels, int size, int x1, int y1, int x2, int y2, int x3, int y3)
    {
        float minX = Mathf.Min(x1, Mathf.Min(x2, x3));
        float maxX = Mathf.Max(x1, Mathf.Max(x2, x3));
        float minY = Mathf.Min(y1, Mathf.Min(y2, y3));
        float maxY = Mathf.Max(y1, Mathf.Max(y2, y3));
        for (int y = (int)minY; y <= maxY; y++)
            for (int x = (int)minX; x <= maxX; x++)
            {
                float a = Edge(x1, y1, x2, y2, x, y);
                float b = Edge(x2, y2, x3, y3, x, y);
                float c = Edge(x3, y3, x1, y1, x, y);
                if ((a >= 0f && b >= 0f && c >= 0f) || (a <= 0f && b <= 0f && c <= 0f))
                    pixels[y * size + x] = Color.white;
            }
    }

    private static float Edge(float ax, float ay, float bx, float by, float px, float py)
    {
        return (px - ax) * (by - ay) - (py - ay) * (bx - ax);
    }

    private static void DrawArc(Color[] pixels, int size, int centerX, int centerY, int innerRadius, int outerRadius)
    {
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                float radius = Mathf.Sqrt(dx * dx + dy * dy);
                if (dx > 0 && Mathf.Abs(dy) < dx * 0.85f && radius >= innerRadius && radius <= outerRadius)
                    pixels[y * size + x] = Color.white;
            }
    }
}
