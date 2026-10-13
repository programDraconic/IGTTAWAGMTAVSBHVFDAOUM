public class OrbitingSummon : Summon
{
    public OrbitingSummon() : base("orbiting")
    {
    }

    public override void UpdateBehavior(SummonContext context)
    {
        LastBehavior = "Orbiting around the player";
    }
}
