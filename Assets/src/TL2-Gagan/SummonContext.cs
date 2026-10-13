using UnityEngine;

// Carries the small amount of game information needed by a summon update.
public class SummonContext
{
    public Vector2 PlayerPosition { get; }
    public string ValidTargetId { get; }
    public bool RevengeTriggered { get; }

    public SummonContext(
        Vector2 playerPosition,
        string validTargetId = null,
        bool revengeTriggered = false)
    {
        PlayerPosition = playerPosition;
        ValidTargetId = validTargetId;
        RevengeTriggered = revengeTriggered;
    }
}
