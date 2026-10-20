// Assets/src/TL5-Wyatt/Potential.cs
// Subclass from the TL5 class diagram - a recruitable shrimp courted during the run.

public class Potential : Suitor
{
    public Potential() : base("potential", "Potential Recruit") { }

    protected override int EvaluateTaste(Gift gift)
    {
        // Vampire tastes: blood above all, indifferent to the rest.
        if (gift.Kind == GiftKind.Blood) return gift.Value + 3;
        return gift.Value - 1;
    }

    protected override Reaction SelectReaction()
    {
        if (State.Stage >= RelationshipStage.Approving)
            return new Reaction("\"I will fight beside you. Point me at something.\"", Mood.Delighted);
        if (State.Affection <= 0)
            return new Reaction("\"I do not know you well enough for this.\"", Mood.Annoyed);
        return new Reaction("\"...Go on, then. I am listening.\"", Mood.Neutral);
    }
}
