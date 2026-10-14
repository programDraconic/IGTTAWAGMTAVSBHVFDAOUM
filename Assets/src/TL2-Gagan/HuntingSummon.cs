public class HuntingSummon : Summon
{
    public HuntingSummon() : base("hunting")
    {
    }

    public override void UpdateBehavior(SummonContext context)
    {
        if (string.IsNullOrEmpty(context.ValidTargetId))
        {
            LastBehavior = "Hunting: no valid target";
            return;
        }

        LastBehavior = "Hunting target: " + context.ValidTargetId;
    }
}
