using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class LocalWindowsBuild
{
    public static void Build()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = "C:/unityfiles/VillanGame/Builds/Windows/VillanGame.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"Windows build result: {report.summary.result}; output: {options.locationPathName}");
        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes;
        var paths = new System.Collections.Generic.List<string>();
        foreach (var scene in scenes)
        {
            if (scene.enabled)
                paths.Add(scene.path);
        }

        return paths.ToArray();
    }
}
