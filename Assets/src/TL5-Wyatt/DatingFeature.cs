using UnityEngine;

public class DatingFeature : MonoBehaviour, IGameplayFeature
{
    private static RelationshipCarryOver carryOver;

    private RunSession session;

    public DatingController Controller { get; private set; }
    public float PhaseSeconds { get; private set; }
    public int UnlocksThisVisit { get; private set; }

    private void Awake()
    {
        Controller = new DatingController();
        Controller.EnterDatingPhase();
        Controller.UnlockEarned += OnUnlockEarned;

        if (carryOver != null) carryOver.RestoreInto(Controller);
    }

    private void Start()
    {
        RunSessionRunner runner = RunSessionRunner.Instance;
        if (runner == null)
        {
            Debug.Log("DatingFeature: no RunSessionRunner in this scene, running standalone.");
            return;
        }

        session = runner.Session;
        session.Registry.RegisterFeature(this);
    }

    private void OnDestroy()
    {
        Capture();
        if (Controller != null) Controller.UnlockEarned -= OnUnlockEarned;
        if (session != null) session.Registry.UnregisterFeature(this);
    }

    private void OnUnlockEarned(RelationshipUnlock unlock)
    {
        UnlocksThisVisit++;
        Debug.Log("[F6 -> F3] " + unlock);
    }

    public void Capture()
    {
        if (Controller == null) return;
        if (carryOver == null) carryOver = new RelationshipCarryOver();
        carryOver.CaptureFrom(Controller);
    }

    public static void ClearCarryOver()
    {
        carryOver = null;
    }

    public bool Initialize(object context)
    {
        session = context as RunSession;
        return Controller != null;
    }

    public void Tick(FrameContext frame)
    {
        PhaseSeconds += frame.DeltaTime;
    }

    public void Shutdown()
    {
        Capture();
    }
}
