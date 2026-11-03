using System.Collections;
using NUnit.Framework;
using TL6.PlayerCore;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthSceneTests
{
    [UnityTearDown]
    public IEnumerator LeavePlayMode()
    {
        if (Application.isPlaying) yield return new ExitPlayMode();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator SavedDemo_UsesSharedHealthAndPublishesDeath()
    {
        EditorSceneManager.OpenScene("Assets/tst/TL6-Nishant/PlayerHealthDemo.unity");
        yield return new EnterPlayMode();
        yield return null;

        var health = Object.FindAnyObjectByType<PlayerHealthController>();
        Assert.That(health, Is.Not.Null);
        Assert.That(health.CurrentHP, Is.EqualTo(100));
        Assert.That(PlayerStats.Shared.Health, Is.EqualTo(100));

        Assert.That(health.ApplyDamage(10), Is.EqualTo(10));
        Assert.That(PlayerStats.Shared.Health, Is.EqualTo(90));
        Assert.That(health.Heal(20), Is.EqualTo(10));

        int deaths = 0;
        health.PlayerDied += () => deaths++;
        health.Initialise();
        health.ApplyDamage(1000);
        health.ApplyDamage(1000);
        Assert.That(deaths, Is.EqualTo(1));
        Assert.That(PlayerStats.Shared.Health, Is.Zero);
    }
}
