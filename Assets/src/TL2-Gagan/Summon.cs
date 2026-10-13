// Plain C# superclass. It intentionally does not inherit from MonoBehaviour.
public abstract class Summon
{
    public string SummonId { get; }
    public bool Active { get; private set; }
    public string LastBehavior { get; protected set; }

    protected Summon(string summonId)
    {
        SummonId = summonId;
        LastBehavior = "Not updated yet";
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }

    // Dynamic-binding point. Each summon subclass overrides this method.
    public virtual void UpdateBehavior(SummonContext context)
    {
        LastBehavior = "Base summon behavior";
    }
}
