// Assets/src/TL5-Wyatt/Suitor.cs
// Abstract superclass from the TL5 class diagram (slides 34-36).
// Plain C# - NOT a MonoBehaviour, so tests can construct subclasses with 'new'.

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

    // Read-only view for the UI and for F3's persistence. The field itself stays private.
    public RelationshipState State { get { return relationshipState; } }

    // ---- TEMPLATE METHOD -------------------------------------------------
    // Fixed three-step courtship sequence. Deliberately NOT virtual: every suitor
    // runs the same steps, and only the two steps below may vary by type.
    public Reaction ReceiveGift(Gift gift)
    {
        int delta = EvaluateTaste(gift);          // step 1 - overridable
        relationshipState.Record(gift, delta);    // step 2 - fixed, identical for all
        return SelectReaction();                  // step 3 - overridable
    }

    // "Begin Conversation" use case: a reaction with no gift involved.
    public Reaction Converse()
    {
        return SelectReaction();
    }

    // ---- THE DYNAMIC BINDING PAIR ---------------------------------------
    // virtual here, override in Husband / Matriarch / Potential / Friend.
    protected virtual int EvaluateTaste(Gift gift)
    {
        return gift.Value;
    }

    protected virtual Reaction SelectReaction()
    {
        return new Reaction(DisplayName + " accepts it without comment.", Mood.Neutral);
    }
}
