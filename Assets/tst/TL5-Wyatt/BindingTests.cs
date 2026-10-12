

using NUnit.Framework;

public class BindingTests
{

    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        SortAlgorithm baseline = new SortAlgorithm();
        SortAlgorithm actual   = new SortAscending();

        string a = baseline.Sort();                  
        string b = actual.Sort();

        Assert.AreNotEqual(a, b);                    
    }


    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        SortAlgorithm baseline = new SortAlgorithm();
        SortAlgorithm actual   = new SortAlgorithm();



        Assert.AreEqual(baseline.Sort(), actual.Sort());
    }
}
