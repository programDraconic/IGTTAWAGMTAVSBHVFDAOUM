// Assets/src/TL5-Wyatt/RelationshipState.cs
// "stores" association from Suitor on the TL5 class diagram.
// Fields match the diagram exactly: affection, stage, giftHistory.

using System.Collections.Generic;

public class RelationshipState
{
    private int affection;
    private RelationshipStage stage;
    private readonly List<Gift> giftHistory = new List<Gift>();

    // Affection needed to reach each stage, indexed by RelationshipStage.
    private static readonly int[] Thresholds = { 0, 4, 10, 18, 28 };

    public int Affection { get { return affection; } }
    public RelationshipStage Stage { get { return stage; } }
    public IReadOnlyList<Gift> GiftHistory { get { return giftHistory; } }

    // Fixed step of the ReceiveGift template - identical for every suitor.
    public void Record(Gift gift, int delta)
    {
        giftHistory.Add(gift);
        affection += delta;
        if (affection < 0) affection = 0;
    }

    // "Advance Relationship" use case. Returns true if the stage actually moved up.
    public bool TryAdvanceStage()
    {
        int next = (int)stage + 1;
        if (next > (int)RelationshipStage.Betrothed) return false;
        if (affection < Thresholds[next]) return false;
        stage = (RelationshipStage)next;
        return true;
    }

    public int AffectionToNextStage()
    {
        int next = (int)stage + 1;
        if (next > (int)RelationshipStage.Betrothed) return 0;
        int needed = Thresholds[next] - affection;
        return needed < 0 ? 0 : needed;
    }

    // "Capture / Restore State" use case - the surface F3 persists.
    public RelationshipSnapshot Capture()
    {
        return new RelationshipSnapshot(affection, stage, giftHistory.Count);
    }

    public void Restore(RelationshipSnapshot snapshot)
    {
        affection = snapshot.Affection;
        stage = snapshot.Stage;
        giftHistory.Clear();
    }
}

public class RelationshipSnapshot
{
    public readonly int Affection;
    public readonly RelationshipStage Stage;
    public readonly int GiftsGiven;

    public RelationshipSnapshot(int affection, RelationshipStage stage, int giftsGiven)
    {
        Affection = affection;
        Stage = stage;
        GiftsGiven = giftsGiven;
    }
}
