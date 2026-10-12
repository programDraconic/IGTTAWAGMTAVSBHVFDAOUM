using NUnit.Framework;

public class MichaelBindingTests
{
    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Enemy baseline = new Enemy();
        Enemy actual = new Enemy();

        Assert.AreEqual(baseline.Attack(), actual.Attack());
    }
}