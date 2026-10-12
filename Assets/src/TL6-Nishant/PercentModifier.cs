public class PercentModifier : StatModifier
{
    public override float Apply(float current)
    {
        return current * 1.2f;
    }
}