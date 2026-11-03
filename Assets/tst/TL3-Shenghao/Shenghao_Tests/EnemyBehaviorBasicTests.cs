using NUnit.Framework;
using ShrimpGame;
using UnityEngine;

public class EnemyBehaviorBasicTests
{
    private GameObject enemyObject;
    private GameObject playerObject;
    private EnemyBehavior enemy;

    [SetUp]
    public void SetUp()
    {
        enemyObject = new GameObject("Test Enemy");
        playerObject = new GameObject("Test Player");
        enemy = enemyObject.AddComponent<EnemyBehavior>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void UnknownWeapon_DealsBaseDamage()
    {
        float damage = enemy.GetDamageFromWeapon("unknown", 10f);

        Assert.AreEqual(10f, damage);
    }

    [Test]
    public void PlayerInsideRange_EnemyChasesAtDefaultSpeed()
    {
        playerObject.transform.position = new Vector3(3f, 0f, 0f);

        bool chasing = enemy.TryGetChaseVelocity(
            playerObject.transform, out Vector2 velocity);

        Assert.IsTrue(chasing);
        Assert.AreEqual(2f, velocity.x, 0.001f);
        Assert.AreEqual(0f, velocity.y, 0.001f);
    }

    [Test]
    public void PlayerOutsideRange_EnemyDoesNotChase()
    {
        playerObject.transform.position = new Vector3(6f, 0f, 0f);

        bool chasing = enemy.TryGetChaseVelocity(
            playerObject.transform, out Vector2 velocity);

        Assert.IsFalse(chasing);
        Assert.AreEqual(Vector2.zero, velocity);
    }
}