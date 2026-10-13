using NUnit.Framework;
using UnityEngine;

public class BindingTests
{
    private SummonContext context;

    [SetUp]
    public void SetUp()
    {
        context = new SummonContext(
            Vector2.zero,
            "Enemy-01",
            true);
    }

    [Test]
    public void BaseReference_ToOrbitingSummon_UsesOrbitingOverride()
    {
        Summon summon = new OrbitingSummon();

        summon.UpdateBehavior(context);

        Assert.AreEqual("Orbiting around the player", summon.LastBehavior);
    }

    [Test]
    public void BaseReference_ToHuntingSummon_UsesHuntingOverride()
    {
        Summon summon = new HuntingSummon();

        summon.UpdateBehavior(context);

        Assert.AreEqual("Hunting target: Enemy-01", summon.LastBehavior);
    }

    [Test]
    public void BaseReference_ToTotemSummon_UsesTotemOverride()
    {
        Summon summon = new TotemSummon();

        summon.UpdateBehavior(context);

        Assert.AreEqual(
            "Totem holding its deployment position",
            summon.LastBehavior);
    }

    [Test]
    public void BaseReference_ToRevengeSummon_UsesRevengeOverride()
    {
        Summon summon = new RevengeSummon();

        summon.UpdateBehavior(context);

        Assert.AreEqual("Revenge response triggered", summon.LastBehavior);
    }

    [Test]
    public void HuntingSummon_WithNoTarget_UsesSafeIdleBehavior()
    {
        Summon summon = new HuntingSummon();
        SummonContext noTargetContext = new SummonContext(Vector2.zero);

        summon.UpdateBehavior(noTargetContext);

        Assert.AreEqual("Hunting: no valid target", summon.LastBehavior);
    }
}
