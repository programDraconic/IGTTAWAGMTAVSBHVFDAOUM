using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DatingStressSetup
{
    public const string ScenePath = "Assets/tst/TL5-Wyatt/TL5_Dating_Stress.unity";

    [MenuItem("TL5/Create Dating Stress Scene")]
    public static void CreateScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.04f, 0.03f, 0.07f);
        cameraObject.AddComponent<AudioListener>();

        AddWall("Left wall", new Vector2(-8.75f, 0f), new Vector2(0.5f, 10f));
        AddWall("Right wall", new Vector2(8.75f, 0f), new Vector2(0.5f, 10f));
        AddWall("Bottom wall", new Vector2(0f, -4.75f), new Vector2(18f, 0.5f));
        AddWall("Top wall", new Vector2(0f, 4.75f), new Vector2(18f, 0.5f));

        GameObject runner = new GameObject("Dating Stress Test");
        runner.AddComponent<DatingStressTest>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("TL5 dating stress scene created at " + ScenePath);
    }

    [MenuItem("TL5/Open Dating Stress Scene")]
    public static void OpenScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(ScenePath);
    }

    private static void AddWall(string wallName, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.position = position;
        BoxCollider2D box = wall.AddComponent<BoxCollider2D>();
        box.size = size;
    }
}
