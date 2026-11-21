using System.Collections.Generic;
using UnityEngine;

/// <summary>Tracks modal gameplay popups so the global controls stay behind them and cannot receive input.</summary>
public static class GameplayPopupState
{
    private static readonly HashSet<Object> OpenPopups = new HashSet<Object>();

    public static bool IsAnyOpen => OpenPopups.Count > 0;

    public static void SetOpen(Object owner, bool isOpen)
    {
        if (owner == null) return;
        if (isOpen) OpenPopups.Add(owner);
        else OpenPopups.Remove(owner);
        HomeButtonBootstrap.RefreshPopupState();
    }

    public static void Clear()
    {
        OpenPopups.Clear();
        HomeButtonBootstrap.RefreshPopupState();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => OpenPopups.Clear();
}
