public class Matriarch : Suitor
{
    public Matriarch() : base("matriarch", "The Matriarch") { }

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
                return new Reaction("\"You are not family. Do not pretend otherwise.\"", Mood.Insulted);
            case RelationshipStage.Tolerating:
                return new Reaction("\"Hm. It is not nothing.\"", Mood.Annoyed);
            case RelationshipStage.Betrothed:
                return new Reaction("\"...You may call me Mother. Once.\"", Mood.Pleased);
            default:
                return new Reaction("\"My son could have done worse. Marginally.\"", Mood.Neutral);
        }
    }
}
