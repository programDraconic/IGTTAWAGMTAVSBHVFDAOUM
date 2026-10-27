using System.Collections;
using NUnit.Framework;
using TL6;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerCoreStressSceneTests
{
    [UnityTearDown]
    public IEnumerator LeavePlayMode()
    {
        if (Application.isPlaying) yield return new ExitPlayMode();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator SavedScene_SpawnsMovesMeasuresAndClears()
    {
        EditorSceneManager.OpenScene("Assets/tst/TL6-Nishant/TL6_PlayerCore_Stress.unity");
        yield return new EnterPlayMode();
        yield return null;

        PlayerCoreStressTest test = Object.FindAnyObjectByType<PlayerCoreStressTest>();
        Assert.That(test, Is.Not.Null);
        Assert.That(test.InstanceCount, Is.EqualTo(1));
        PlayerCoreStressBody first = test.GetComponentInChildren<PlayerCoreStressBody>();
        Vector3 before = first.transform.position;
        float movementDeadline = Time.realtimeSinceStartup + 3f;
        while (Vector3.Distance(before, first.transform.position) < 0.01f &&
               Time.realtimeSinceStartup < movementDeadline) yield return null;
        Assert.That(Vector3.Distance(before, first.transform.position), Is.GreaterThan(0.001f));

        test.Spawn(99);
        Assert.That(test.InstanceCount, Is.EqualTo(100));
        test.Clear();
        yield return null;
        Assert.That(test.GetComponentsInChildren<PlayerCoreStressBody>(), Is.Empty);

        Assert.That(test.InstanceCount, Is.Zero);

        SerializedObject settings = new SerializedObject(test);
        settings.FindProperty("maximumObjects").intValue = 20;
        settings.FindProperty("sampleSeconds").floatValue = 0.5f;
        settings.FindProperty("settleSeconds").floatValue = 0.1f;
        settings.ApplyModifiedPropertiesWithoutUndo();
        test.StartTest();
        float deadline = Time.realtimeSinceStartup + 10f;
        while (test.IsRunning && Time.realtimeSinceStartup < deadline) yield return null;

        Assert.That(test.IsRunning, Is.False, "The test must stop at its object limit or FPS break.");
        Assert.That(test.StartingFps, Is.GreaterThan(0f));
        Assert.That(test.StopAtFps, Is.EqualTo(test.StartingFps / 2f));
        Assert.That(test.InstanceCount, Is.EqualTo(20));
        Assert.That(test.FpsBreakCount, Is.EqualTo(0).Or.EqualTo(20));
        test.Clear();
        yield return null;
        Assert.That(test.GetComponentsInChildren<PlayerCoreStressBody>(), Is.Empty);
    }
}
