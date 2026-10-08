using NUnit.Framework;

public class BindingTests
{

    RunSession session = new RunSession();
    FrameContext frame = new FrameContext();
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        RunState baseline = new BaseOnlyState();    // ARRANGING OBJECTS
        RunState actual = new PlayingState();
        string a = baseline.Tick(session, frame);                 // ACTIONS (ON OBJECTS)
        string b = actual.Tick(session, frame);
        Assert.AreNotEqual(a, b);                   // ASSERT - DIFFERENT
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        RunState baseline = new BaseOnlyState();    // ARRANGING OBJECTS
        RunState actual = new BaseOnlyState();
        string a = baseline.Tick(session, frame);                 // ACTIONS (ON OBJECTS)
        string b = actual.Tick(session, frame);
        Assert.AreEqual(a, b);                   // ASSERT - DIFFERENT        
    }
}
