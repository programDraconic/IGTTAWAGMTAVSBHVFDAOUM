using UnityEngine;

public class SummonContext
{
    public Vector2 PlayerPosition { get; }
    public string ValidTargetId { get; }
    public bool RevengeTriggered { get; }
    public float DeltaTime { get; }

    public SummonContext(
        Vector2 playerPosition,
        string validTargetId = null,
        bool revengeTriggered = false,
        float deltaTime = 0f)
    {
        PlayerPosition = playerPosition;
        ValidTargetId = validTargetId;
        RevengeTriggered = revengeTriggered;
        DeltaTime = Mathf.Max(0f, deltaTime);
    }
}