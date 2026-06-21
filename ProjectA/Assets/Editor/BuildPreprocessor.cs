// Assets/Editor/BuildPreprocessor.cs
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 빌드 전에 화면 방향을 가로 고정으로 강제.
/// Player Settings가 실수로 변경되어도 CI 빌드에서 자동 보정됨.
/// </summary>
public class BuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("[BuildPreprocessor] 화면 방향을 가로 고정으로 설정");

        // 기본 방향: 가로 (왼쪽)
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        // Auto Rotation 허용 방향 — 세로 차단, 가로만 허용
        PlayerSettings.allowedAutorotateToPortrait          = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft      = true;
        PlayerSettings.allowedAutorotateToLandscapeRight     = true;

        // 자동 회전 자체는 켜두고 (가로 양방향 허용)
        PlayerSettings.useAnimatedAutorotation = true;

        Debug.Log("[BuildPreprocessor] 화면 방향 설정 완료: Landscape Only");
    }
}