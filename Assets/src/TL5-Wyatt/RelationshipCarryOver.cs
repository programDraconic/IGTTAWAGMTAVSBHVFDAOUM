using System.Collections.Generic;

public class RelationshipCarryOver
{
    private Dictionary<string, RelationshipSnapshot> snapshot;

    public bool HasData { get { return snapshot != null && snapshot.Count > 0; } }

    public void CaptureFrom(DatingController controller)
    {
        if (controller == null) return;
        snapshot = controller.CaptureState();
    }

    public void RestoreInto(DatingController controller)
    {
        if (controller == null || !HasData) return;
        controller.RestoreState(snapshot);
    }
}
