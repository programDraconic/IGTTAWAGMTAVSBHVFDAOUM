public class RevengeSummon : Summon
{
    public RevengeSummon() : base("revenge")
    {
    }

    public override void UpdateBehavior(SummonContext context)
    {
        LastBehavior = context.RevengeTriggered
            ? "Revenge response triggered"
            : "Revenge waiting for its trigger";
    }
}
