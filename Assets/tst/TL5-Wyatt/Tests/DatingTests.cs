using NUnit.Framework;

public class DatingTests
{
    [Test]
    public void BaseRef_ToDifferentSubclasses_UsesOverride()
    {
        Suitor matriarch = new Matriarch();
        Suitor friend    = new Friend();

        Reaction a = matriarch.ReceiveGift(Gift.ShrimpCracker);
        Reaction b = friend.ReceiveGift(Gift.ShrimpCracker);

        Assert.AreNotEqual(friend.State.Affection, matriarch.State.Affection);
        Assert.AreNotEqual(a.Line, b.Line);
    }

    [Test]
    public void BaseRef_ToSameSubclass_NoBindingDifference()
    {
        Suitor one = new Friend();
        Suitor two = new Friend();

        Reaction a = one.ReceiveGift(Gift.ShrimpCracker);
        Reaction b = two.ReceiveGift(Gift.ShrimpCracker);

        Assert.AreEqual(one.State.Affection, two.State.Affection);
        Assert.AreEqual(a.Line, b.Line);
    }
}
