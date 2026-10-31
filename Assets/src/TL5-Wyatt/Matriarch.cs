public class Matriarch : Suitor
{
    public Matriarch() : base("matriarch", "The Racist Shrimp Mom") { }

    protected override int EvaluateTaste(Gift gift)
    {
        if (gift.Kind == GiftKind.Heirloom) return gift.Value + 4;
        if (gift.Kind == GiftKind.Trinket)  return -3;
        return 1;
    }

    protected override Reaction SelectReaction()
    {
        switch (State.Stage)
        {
            case RelationshipStage.Hostile:
                return new Reaction("\"You are shrimpgusting. Do not pretend otherwise.\"", Mood.Insulted);
            case RelationshipStage.Tolerating:
                return new Reaction("\"Hm. It is shrimpthing.\"", Mood.Annoyed);
            case RelationshipStage.Betrothed:
                return new Reaction("\"...You may call me mothershrimper. Once.\"", Mood.Pleased);
            default:
                return new Reaction("\"I suppose my shrimpson could have done worse.\"", Mood.Neutral);
        }
    }
}
