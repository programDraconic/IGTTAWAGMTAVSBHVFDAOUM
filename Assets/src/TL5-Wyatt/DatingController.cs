using System;
using System.Collections.Generic;

public class DatingController
{
    private readonly List<Suitor> suitors = new List<Suitor>();
    private int current;

    public event Action<RelationshipUnlock> UnlockEarned;

    private static readonly Dictionary<RelationshipStage, string> StageRewards =
        new Dictionary<RelationshipStage, string>
        {
            { RelationshipStage.Tolerating, "Permanent bonus: +5 max HP" },
            { RelationshipStage.Warm,       "Summon unlocked: Orbiting Familiar" },
            { RelationshipStage.Approving,  "Permanent bonus: +10% Might" },
            { RelationshipStage.Betrothed,  "Summon unlocked: Hunting Summon" },
        };

    public void EnterDatingPhase()
    {
        if (suitors.Count == 0)
        {
            suitors.Add(new Husband());
            suitors.Add(new Matriarch());
            suitors.Add(new Potential());
            suitors.Add(new Friend());
        }
        current = 0;
    }

    public Suitor[] GetSuitors()
    {
        return suitors.ToArray();
    }

    public Suitor CurrentSuitor
    {
        get { return suitors.Count == 0 ? null : suitors[current]; }
    }

    public void SelectSuitor(int index)
    {
        if (index < 0 || index >= suitors.Count) return;
        current = index;
    }

    public Reaction OfferGift(Gift gift)
    {
        Suitor suitor = CurrentSuitor;
        if (suitor == null || gift == null) return null;
        return suitor.ReceiveGift(gift);
    }

    public Reaction BeginConversation()
    {
        Suitor suitor = CurrentSuitor;
        return suitor == null ? null : suitor.Converse();
    }

    public bool Advance()
    {
        Suitor suitor = CurrentSuitor;
        if (suitor == null) return false;
        if (!suitor.State.TryAdvanceStage()) return false;

        string reward;
        if (StageRewards.TryGetValue(suitor.State.Stage, out reward))
        {
            RelationshipUnlock unlock = new RelationshipUnlock(suitor.Id, suitor.State.Stage, reward);
            if (UnlockEarned != null) UnlockEarned(unlock);
        }
        return true;
    }

    public Dictionary<string, RelationshipSnapshot> CaptureState()
    {
        var snapshot = new Dictionary<string, RelationshipSnapshot>();
        foreach (Suitor s in suitors) snapshot[s.Id] = s.State.Capture();
        return snapshot;
    }

    public void RestoreState(Dictionary<string, RelationshipSnapshot> snapshot)
    {
        if (snapshot == null) return;
        EnterDatingPhase();
        foreach (Suitor s in suitors)
        {
            RelationshipSnapshot saved;
            if (snapshot.TryGetValue(s.Id, out saved)) s.State.Restore(saved);
        }
    }
}
