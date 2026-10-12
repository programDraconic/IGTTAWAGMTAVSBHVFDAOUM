using NUnit.Framework;
public class BindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        Enemy baseline = new Enemy();         // arrange
        Enemy actual = new Dragon();
        string a = baseline.Attack();         // act
        string b = actual.Attack();
        Assert.AreNotEqual(a, b);             // assert: DIFFERENT
    }
}
