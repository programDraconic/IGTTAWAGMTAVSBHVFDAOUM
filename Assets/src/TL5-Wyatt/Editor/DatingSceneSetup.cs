using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DatingSceneSetup
{
    public const string ScenePath = "Assets/scenes/S3-S7-Dating.unity";
    public const string RootName = "Dating System";

    [MenuItem("TL5/Build Dating Into S3-S7-Dating Scene")]
    public static void Build()
    {
        if (!File.Exists(ScenePath))
        {
            Debug.LogError("DatingSceneSetup: " + ScenePath + " not found.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene scene = EditorSceneManager.OpenScene(ScenePath);

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == RootName)
            {
                Debug.LogWarning("DatingSceneSetup: '" + RootName + "' is already in " + ScenePath +
                                 ". Nothing was changed. Delete that object first if you want it rebuilt.");
                Selection.activeGameObject = root;
                return;
            }
        }

        GameObject rig = new GameObject(RootName);
        rig.AddComponent<DatingFeature>();
        rig.AddComponent<DatingSceneController>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = rig;
        Debug.Log("DatingSceneSetup: added '" + RootName + "' to " + ScenePath + ".");
    }
}
