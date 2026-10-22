public class Husband : Suitor
{
    public Husband() : base("husband", "Vampire Shrimp Husband") { }

    protected override int EvaluateTaste(Gift gift)
    {
        if (gift.Kind == GiftKind.Delicacy) return gift.Value + 3;
        return gift.Value + 1;
    }

    protected override Reaction SelectReaction()
    {
        switch (State.Stage)
        {
            case RelationshipStage.Betrothed:
                return new Reaction("\"My family will come around. They have to.\"", Mood.Delighted);
            case RelationshipStage.Approving:
                return new Reaction("\"You are winning them over. I knew you would.\"", Mood.Pleased);
            default:
                return new Reaction("\"You did not have to. But I am glad you did.\"", Mood.Pleased);
        }
    }
}
