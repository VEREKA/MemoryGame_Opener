#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Text;

public static class EditorDiagnostics
{
    [MenuItem("Tools/Editor Diagnostics/Find Missing Scripts in Open Scenes")]
    public static void FindMissingScriptsInOpenScenes()
    {
        var sb = new StringBuilder();
        int sceneCount = EditorSceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            var rootObjects = scene.GetRootGameObjects();
            foreach (var go in rootObjects)
            {
                CheckGameObjectRecursive(go, sb);
            }
        }

        if (sb.Length == 0)
            Debug.Log("No missing scripts found in open scenes.");
        else
            Debug.Log(sb.ToString());
    }

    static void CheckGameObjectRecursive(GameObject go, StringBuilder sb)
    {
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                sb.AppendLine($"Missing script on GameObject: {GetPath(go)}");
            }
            else
            {
                try
                {
                    SerializedObject so = new SerializedObject(components[i]);
                    SerializedProperty prop = so.GetIterator();
                    while (prop.NextVisible(true))
                    {
                        if (prop.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                            {
                                sb.AppendLine($"Null reference field in {GetPath(go)} component {components[i].GetType().Name}: {prop.name}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    sb.AppendLine($"SerializedObject error on {GetPath(go)} component {components[i].GetType().Name}: {ex.Message}");
                }
            }
        }

        foreach (Transform child in go.transform)
            CheckGameObjectRecursive(child.gameObject, sb);
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

    [MenuItem("Tools/Editor Diagnostics/Run All Diagnostics")]
    public static void RunAll()
    {
        FindMissingScriptsInOpenScenes();
        Debug.Log("Editor diagnostics finished.");
    }
}
#endif