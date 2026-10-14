public class TotemSummon : Summon
{
    public TotemSummon() : base("totem")
    {
    }

    public override void UpdateBehavior(SummonContext context)
    {
        LastBehavior = "Totem holding its deployment position";
    }
}
