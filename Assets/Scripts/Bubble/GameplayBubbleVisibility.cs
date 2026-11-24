public static class GameplayBubbleVisibility
{
    public static void HideAll()
    {
        if (BubbleManager.Instance != null)
            BubbleManager.Instance.HideBubble();

        if (SubwayBubbleManager.Instance != null)
            SubwayBubbleManager.Instance.HideBubble();
    }
}
