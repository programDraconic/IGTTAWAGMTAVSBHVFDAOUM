using UnityEngine;

public class OrbitingSummon : Summon
{
    private readonly float radius;
    private readonly float degreesPerSecond;
    private float angleDegrees;

    public Vector2 Position { get; private set; }

    public OrbitingSummon(
        float radius = 2f,
        float degreesPerSecond = 90f) : base("orbiting")
    {
        this.radius = Mathf.Max(0f, radius);
        this.degreesPerSecond = degreesPerSecond;
    }

    public override void UpdateBehavior(SummonContext context)
    {
        LastBehavior = "Orbiting around the player";

        if (context == null)
            return;

        angleDegrees = Mathf.Repeat(
            angleDegrees + degreesPerSecond * context.DeltaTime,
            360f);

        float radians = angleDegrees * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)) * radius;

        Position = context.PlayerPosition + offset;
    }
}