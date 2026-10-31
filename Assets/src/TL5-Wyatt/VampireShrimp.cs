public class VampireShrimp : Suitor
{
    public VampireShrimp() : base("vampireshrimp", "The Vampire Shrimp") { }

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
                return new Reaction("\"My family's so dumb, you're winning them over! I knew you would.\"", Mood.Pleased);
            default:
                return new Reaction("\"I don't know why you would do that, but I am glad you did.\"", Mood.Pleased);
        }
    }
}
