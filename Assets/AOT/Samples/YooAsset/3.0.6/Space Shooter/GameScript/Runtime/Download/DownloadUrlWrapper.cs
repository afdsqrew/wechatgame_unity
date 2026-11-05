using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadUrlWrapper
{
    bool useLocalUrlCdnFlag= false;
    public string GetDownloadUrl()
    {
        if (useLocalUrlCdnFlag)
        {
            return
                $"http://192.168.1.6/v1.0.0";
        }
        else
        {
#if UNITY_EDITOR
            

            string hostServerIP =
                "https://pub-c23bd805be32414d86b1a214a8dcfdf4.r2.dev";
            string appVersion = "v1.0.0";

            if (UnityEditor.EditorUserBuildSettings
                    .activeBuildTarget == UnityEditor
                    .BuildTarget.Android)
                return
                    $"{hostServerIP}/CDN/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings
                         .activeBuildTarget == UnityEditor
                         .BuildTarget.iOS)
                return
                    $"{hostServerIP}/CDN/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings
                         .activeBuildTarget == UnityEditor
                         .BuildTarget.WebGL)
                return
                    $"{hostServerIP}/{appVersion}/yoo/DefaultPackage";
            else
                return
                    $"{hostServerIP}/CDN/PC/{appVersion}";
            #else
                return "";
            #endif
        }
    }

    public void SetUseLocalCDNFlag(bool useLocalUrlCdn)
    {
        useLocalUrlCdnFlag = useLocalUrlCdn;
    }
}
