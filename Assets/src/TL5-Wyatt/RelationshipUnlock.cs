// Assets/src/TL5-Wyatt/RelationshipUnlock.cs
// Payload of the "Publish Unlock Event" use case. Consumed by F3 (Summons & Persistence).

public class RelationshipUnlock
{
    public readonly string SuitorId;
    public readonly RelationshipStage Stage;
    public readonly string Reward;

    public RelationshipUnlock(string suitorId, RelationshipStage stage, string reward)
    {
        SuitorId = suitorId;
        Stage = stage;
        Reward = reward;
    }

    public override string ToString()
    {
        return SuitorId + " reached " + Stage + " -> unlocked " + Reward;
    }
}
