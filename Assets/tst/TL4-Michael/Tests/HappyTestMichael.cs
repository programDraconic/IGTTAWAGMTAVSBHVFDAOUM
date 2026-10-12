using NUnit.Framework;

public class HappyTestMichael
{
    [Test]
    public void BaseRef_ToXPPickup_UsesOverride()
    {
        PlayerStats player = new PlayerStats();
        Pickup baseline = new Pickup();
        Pickup actual = new XPPickup(); // Declared Pickup; runtime XPPickup.

        string baseResult = baseline.Collect(player);
        string actualResult = actual.Collect(player);

        Assert.AreNotEqual(baseResult, actualResult);
        Assert.AreEqual("Collected XP: +25", actualResult);
        Assert.AreEqual(25, player.XP);
    }
}
