using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Builder
{
    public static void BuildProject()
    {
        try
        {
            List<string> ActiveScenePaths = new();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    ActiveScenePaths.Add(scene.path);
                }
            }
            BuildReport buildReport = default;

            string myCustomOption1 = GetArg("myCustomOption1");

            var options = new BuildPlayerOptions
            {
                scenes = ActiveScenePaths.ToArray(),
                target = EditorUserBuildSettings.activeBuildTarget,
                locationPathName = Path.Combine($"build/{EditorUserBuildSettings.activeBuildTarget}", GetBuildTargetOutputFileNameAndExtension()),
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup
            };

            buildReport = BuildPipeline.BuildPlayer(options);

            switch (buildReport.summary.result)
            {
                case BuildResult.Succeeded:
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Unknown:
                case BuildResult.Failed:
                case BuildResult.Cancelled:
                default:
                    EditorApplication.Exit(1);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("BUILD FAILED: " + ex.Message);
            EditorApplication.Exit(1);
        }
    }

    private static string GetBuildTargetOutputFileNameAndExtension()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                return string.Format("{0}.apk", Application.productName);
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows:
                return string.Format("{0}.exe", Application.productName);
#if UNITY_2018_1_OR_NEWER
            case BuildTarget.StandaloneOSX:
#endif
#if !UNITY_2017_3_OR_NEWER
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
#endif
                return string.Format("{0}.app", Application.productName);
            case BuildTarget.iOS:
            case BuildTarget.tvOS:
            case BuildTarget.WebGL:
            case BuildTarget.WSAPlayer:
            case BuildTarget.StandaloneLinux64:
#if !UNITY_2018_3_OR_NEWER
                    case BuildTarget.PSP2:    
#endif
            case BuildTarget.PS4:
            case BuildTarget.XboxOne:
#if !UNITY_2017_3_OR_NEWER
                    case BuildTarget.SamsungTV:
#endif
#if !UNITY_2018_1_OR_NEWER
                    case BuildTarget.N3DS:
                    case BuildTarget.WiiU:
#endif
            case BuildTarget.Switch:
            case BuildTarget.NoTarget:
            default:
                return Application.productName;
        }
    }

    // Helper function for getting the command line arguments
    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }   
}