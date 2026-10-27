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
