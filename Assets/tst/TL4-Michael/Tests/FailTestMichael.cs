using NUnit.Framework;

public class FailTestMichael
{
    [Test]
    public void BaseRef_ToBase_NoOverride()
    {
        PlayerStats player = new PlayerStats();
        Pickup baseline = new Pickup();
        Pickup actual = new Pickup(); // No subclass, so no overridden behavior.

        string baseResult = baseline.Collect(player);
        string actualResult = actual.Collect(player);

        Assert.AreEqual(baseResult, actualResult);
        Assert.AreEqual("Generic pickup collected", actualResult);
        Assert.AreEqual(0, player.XP);
    }
}
