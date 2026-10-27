using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TL6.Editor
{
    public static class PlayerCoreStressSetup
    {
        public const string PrefabPath = "Assets/prefabs/TL6-Nishant/TL6_PlayerCore.prefab";
        public const string ScenePath = "Assets/tst/TL6-Nishant/TL6_PlayerCore_Stress.unity";

        [MenuItem("TL6/Open Player Core Stress Scene")]
        public static void OpenScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
        }

        // Run once to author the assets with Unity's own serializer.
        public static void CreateAssets()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
                throw new InvalidOperationException("The TL6 prefab already exists; generation will not overwrite it.");

            Sprite sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/prefabs/TL6-Nishant/TL6_Shrimp.png")
                .OfType<Sprite>().FirstOrDefault();
            if (sprite == null) throw new InvalidOperationException("GameShrimp sprite could not be loaded.");

            GameObject root = new GameObject("TL6_PlayerCore");
            Rigidbody2D body = root.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;
            root.AddComponent<CircleCollider2D>().radius = 0.18f;
            root.AddComponent<PlayerCoreStressBody>();

            GameObject visual = new GameObject("Shrimp visual");
            visual.transform.SetParent(root.transform, false);
            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            float scale = 0.5f / Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
            visual.transform.localScale = Vector3.one * scale;
            visual.transform.localPosition = -sprite.bounds.center * scale;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.055f, 0.09f);
            cameraObject.AddComponent<AudioListener>();

            GameObject testObject = new GameObject("Stress Test");
            PlayerCoreStressTest test = testObject.AddComponent<PlayerCoreStressTest>();
            SerializedObject serialised = new SerializedObject(test);
            serialised.FindProperty("prefab").objectReferenceValue = prefab.GetComponent<PlayerCoreStressBody>();
            serialised.ApplyModifiedPropertiesWithoutUndo();

            AddWall("Left wall", new Vector2(-8.5f, 0f), new Vector2(0.5f, 9.5f));
            AddWall("Right wall", new Vector2(8.5f, 0f), new Vector2(0.5f, 9.5f));
            AddWall("Bottom wall", new Vector2(0f, -4.5f), new Vector2(17.5f, 0.5f));
            AddWall("Top wall", new Vector2(0f, 4.5f), new Vector2(17.5f, 0.5f));

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("TL6 Player Core prefab and stress scene created.");
        }

        private static void AddWall(string name, Vector2 position, Vector2 size)
        {
            GameObject wall = new GameObject(name);
            wall.transform.position = position;
            wall.AddComponent<BoxCollider2D>().size = size;
        }
    }
}
