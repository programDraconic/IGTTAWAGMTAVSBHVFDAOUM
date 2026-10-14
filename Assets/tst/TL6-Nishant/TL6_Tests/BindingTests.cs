using NUnit.Framework;

public class BindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        StatModifier baseline = new StatModifier();
        StatModifier actual = new PercentModifier();
        float a = baseline.Apply(10f);
        float b = actual.Apply(10f);
        Assert.AreNotEqual(a, b);
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        StatModifier baseline = new StatModifier();
        StatModifier actual = new StatModifier();
        float a = baseline.Apply(10f);
        float b = actual.Apply(10f);
        Assert.AreEqual(a, b);
    }
}
