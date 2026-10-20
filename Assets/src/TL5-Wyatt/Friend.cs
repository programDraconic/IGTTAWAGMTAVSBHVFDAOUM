// Assets/src/TL5-Wyatt/Friend.cs
// Subclass from the TL5 class diagram - the sympathetic in-law. Easiest unlocks.

public class Friend : Suitor
{
    public Friend() : base("friend", "Sympathetic Sibling") { }

    protected override int EvaluateTaste(Gift gift)
    {
        // Low bar. Everything lands, nothing offends.
        return gift.Value + 2;
    }

    protected override Reaction SelectReaction()
    {
        if (State.Stage >= RelationshipStage.Warm)
            return new Reaction("\"Ignore Mother. I am rooting for you both.\"", Mood.Delighted);
        return new Reaction("\"Oh! For me? That is very kind.\"", Mood.Pleased);
    }
}
