public class Friend : Suitor
{
    public Friend() : base("friend", "Sympathetic Shrimp") { }

    protected override int EvaluateTaste(Gift gift)
    {
        return gift.Value + 2;
    }

    protected override Reaction SelectReaction()
    {
        if (State.Stage >= RelationshipStage.Warm)
            return new Reaction("\"*exaggerated shrimp noises*\"", Mood.Delighted);
        return new Reaction("\"*even more exaggerated shrimp noises*\"", Mood.Pleased);
    }
}
