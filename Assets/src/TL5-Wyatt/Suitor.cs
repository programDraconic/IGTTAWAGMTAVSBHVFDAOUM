public abstract class Suitor
{
    private readonly RelationshipState relationshipState = new RelationshipState();

    public readonly string Id;
    public readonly string DisplayName;

    protected Suitor(string id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public RelationshipState State { get { return relationshipState; } }

    public Reaction ReceiveGift(Gift gift)
    {
        int delta = EvaluateTaste(gift);
        relationshipState.Record(gift, delta);
        return SelectReaction();
    }

    public Reaction Converse()
    {
        return SelectReaction();
    }

    protected virtual int EvaluateTaste(Gift gift)
    {
        return gift.Value;
    }

    protected virtual Reaction SelectReaction()
    {
        return new Reaction("...accepts it without comment.", Mood.Neutral);
    }
}
