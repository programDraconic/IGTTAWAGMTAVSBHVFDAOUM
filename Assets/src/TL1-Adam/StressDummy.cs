using UnityEngine;

// Stress-test prefab for TL1: a minimal "feature" that registers with the
// RunSession and does a little work every tick. Thousands of these exercise
// register / unregister / DispatchTick, and prove pausing freezes everything.
public class StressDummy : MonoBehaviour, IGameplayFeature
{
    // Totals for the lecture measurements.
    public static int Alive { get; private set; }
    public static long TotalTicks { get; private set; }

    [SerializeField] private float speed = 2f;
    [SerializeField] private float radius = 3f;

    public int TickCount { get; private set; }   // stops rising while paused

    private RunSession session;
    private Vector3 origin;
    private float phase;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Alive = 0;
        TotalTicks = 0;
    }

    void Start()
    {
        RunSessionRunner runner = RunSessionRunner.Instance;
        if (runner == null)
        {
            Debug.LogWarning("StressDummy: no RunSessionRunner in the scene.");
            enabled = false;
            return;
        }

        session = runner.Session;
        origin = transform.position;
        phase = Random.value * 6.2831853f;
        session.Registry.RegisterFeature(this);
    }

    void OnDestroy()
    {
        if (session != null)
            session.Registry.UnregisterFeature(this);
    }

    // ---- IGameplayFeature ----
    public bool Initialize(object context)
    {
        Alive++;
        return true;
    }

    public void Tick(FrameContext frame)
    {
        TickCount++;
        TotalTicks++;
        phase += speed * frame.DeltaTime;
        transform.position = origin + new Vector3(Mathf.Cos(phase), Mathf.Sin(phase), 0f) * radius;
    }

    public void Shutdown()
    {
        Alive--;
    }
}
