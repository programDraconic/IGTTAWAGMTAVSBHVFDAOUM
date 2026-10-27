using NUnit.Framework;
using TL6;
using UnityEditor;
using UnityEngine;

public class PlayerCorePrefabTests
{
    private const string PrefabPath = "Assets/prefabs/TL6-Nishant/TL6_PlayerCore.prefab";
    private GameObject first;
    private GameObject second;

    [TearDown]
    public void CleanUp()
    {
        if (first != null) Object.DestroyImmediate(first);
        if (second != null) Object.DestroyImmediate(second);
    }

    [Test]
    public void Prefab_InstantiatesWithVisibleSpriteAndDynamicPhysics()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Assert.That(prefab, Is.Not.Null, "The saved prefab must exist.");
        first = Object.Instantiate(prefab);

        Assert.That(first.GetComponent<PlayerCoreStressBody>(), Is.Not.Null);
        Assert.That(first.GetComponent<Rigidbody2D>().bodyType, Is.EqualTo(RigidbodyType2D.Dynamic));
        Assert.That(first.GetComponent<Rigidbody2D>().gravityScale, Is.Zero);
        Assert.That(first.GetComponent<CircleCollider2D>().isTrigger, Is.False);
        Assert.That(first.GetComponentInChildren<SpriteRenderer>().sprite, Is.Not.Null);
    }

    [Test]
    public void TwoInstances_ModifierChangesStayIndependent()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Assert.That(prefab, Is.Not.Null);
        first = Object.Instantiate(prefab);
        second = Object.Instantiate(prefab);
        PlayerCoreStressBody a = first.GetComponent<PlayerCoreStressBody>();
        PlayerCoreStressBody b = second.GetComponent<PlayerCoreStressBody>();
        a.Initialise(Vector2.right, new Vector2(8f, 4f));
        b.Initialise(Vector2.up, new Vector2(8f, 4f));

        a.UsePercentModifier(false);

        Assert.That(a.Might, Is.EqualTo(15f).Within(0.0001f));
        Assert.That(b.Might, Is.EqualTo(12f).Within(0.0001f));
    }
}
