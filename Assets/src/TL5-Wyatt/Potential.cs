public class Potential : Suitor
{
    public Potential() : base("potential", "Potential Recruit") { }

    protected override int EvaluateTaste(Gift gift)
    {
        if (gift.Kind == GiftKind.Blood) return gift.Value + 3;
        return gift.Value - 1;
    }

    protected override Reaction SelectReaction()
    {
        if (State.Stage >= RelationshipStage.Approving)
            return new Reaction("\"My boss sucks anyways, you can be my new boss!\"", Mood.Delighted);
        if (State.Affection <= 0)
            return new Reaction("\"Ew, I don't know you.\"", Mood.Annoyed);
        return new Reaction("\"...Go on, then. I'm somewhat listening.\"", Mood.Neutral);
    }
}
