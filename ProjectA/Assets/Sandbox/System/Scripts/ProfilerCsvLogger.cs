using System.Collections.Generic;
using System.Text;
using System.IO;
using Unity.Profiling;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProfilerCsvLogger : MonoBehaviour
{
    string[] statNames = {
        "Main Thread", "GC.Alloc", "GC.Collect",
        "Batches Count", "SetPass Calls Count", "Triangles Count"
    };
    List<ProfilerRecorder> recorders = new();
    StringBuilder sb = new();

    public string FilePath => Path.Combine(Application.persistentDataPath, "profile.csv");

    void OnEnable()
    {
        sb.Clear();
        sb.AppendLine("frame," + string.Join(",", statNames));
        foreach (var n in statNames)
            recorders.Add(ProfilerRecorder.StartNew(ProfilerCategory.Internal, n));
    }

    void Update()
    {
        sb.Append(Time.frameCount);
        foreach (var r in recorders)
            sb.Append(',').Append(r.Valid ? r.LastValue : 0);
        sb.AppendLine();
    }

    void OnDisable()
    {
        foreach (var r in recorders) r.Dispose();
        recorders.Clear();
        File.WriteAllText(FilePath, sb.ToString());
        Debug.Log($"Saved: {FilePath}");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProfilerCsvLogger))]
public class ProfilerCsvLoggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var logger = (ProfilerCsvLogger)target;
        bool exists = File.Exists(logger.FilePath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save Path", logger.FilePath, EditorStyles.wordWrappedMiniLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Save Location"))
            {
                // 파일이 있으면 그 파일을 폴더에서 하이라이트, 없으면 폴더만 열기
                EditorUtility.RevealInFinder(exists ? logger.FilePath : Application.persistentDataPath);
            }

            using (new EditorGUI.DisabledScope(!exists))
            {
                if (GUILayout.Button("Open CSV File"))
                    EditorUtility.OpenWithDefaultApp(logger.FilePath); // OS 기본 앱으로 열기
            }
        }

        if (!exists)
            EditorGUILayout.HelpBox(
                "CSV가 아직 없습니다. 측정 구간 동안 컴포넌트를 활성화했다가 비활성화하면 저장됩니다.",
                MessageType.Info);
    }
}
#endif