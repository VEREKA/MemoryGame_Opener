#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Linq;

public static class EditorDiagnosticsAssets
{
    [MenuItem("Tools/Editor Diagnostics/Find Issues In Assets (Prefabs)")]
    public static void FindIssuesInPrefabs()
    {
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var sb = new StringBuilder();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) continue;
            CheckGameObjectRecursive(go, sb, path);
        }

        if (sb.Length == 0)
            Debug.Log("No issues found in prefabs.");
        else
            Debug.Log(sb.ToString());
    }

    static void CheckGameObjectRecursive(GameObject go, StringBuilder sb, string assetPath)
    {
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                sb.AppendLine($"Missing script on prefab: {assetPath} GameObject: {GetPath(go)}");
            }
            else
            {
                try
                {
                    SerializedObject so = new SerializedObject(components[i]);
                    // iterate briefly to ensure onEnable of editors doesn't throw
                    var prop = so.GetIterator();
                    while (prop.NextVisible(true)) { }
                }
                catch (System.Exception ex)
                {
                    sb.AppendLine($"SerializedObject error on prefab {assetPath} component {components[i].GetType().Name} at {GetPath(go)}: {ex.Message}");
                }
            }
        }

        foreach (Transform child in go.transform)
            CheckGameObjectRecursive(child.gameObject, sb, assetPath);
    }

    static string GetPath(GameObject go)
    {
        string path = go.name;
        Transform t = go.transform;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
#endif