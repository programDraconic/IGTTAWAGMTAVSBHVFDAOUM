public class FlatModifier : StatModifier
{
    public override float Apply(float current)
    {
        return current + 5f;
    }
}