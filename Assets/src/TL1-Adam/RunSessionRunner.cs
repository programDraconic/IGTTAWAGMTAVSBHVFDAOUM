using UnityEngine;

// The only MonoBehaviour of the run-state system: bridges Unity's Update
// loop to the plain-C# session and freezes Unity while not Playing.
public class RunSessionRunner : MonoBehaviour
{
    public static RunSessionRunner Instance { get; private set; }

    [SerializeField] private float countdownSeconds = 3f;

    [Header("Debug (optional)")]
    [SerializeField] private bool showDebugGui = true;
    [SerializeField] private GameObject stressPrefab;   // drag StressDummy here

    public RunSession Session { get; private set; }

    void Awake()
    {
        Instance = this;
        Session = new RunSession();
        Session.PhaseChanged += OnPhaseChanged;
        RunHandle handle = Session.Initialize(new RunConfig { CountdownSeconds = countdownSeconds });
        Session.StartRun(handle.RunId);
    }

    void Update()
    {
        // Unscaled so the Ready countdown still advances while timeScale is 0.
        Session.Tick(new FrameContext(Time.unscaledDeltaTime, Time.unscaledTime));
    }

    void OnPhaseChanged(RunState state)
    {
        Time.timeScale = state.RunsSimulation ? 1f : 0f;
        AudioListener.pause = !state.RunsSimulation;
        // Show/hide pause menu, level-up panel, HUD here too.
    }

    // Wire this to your pause input (Input System action, UI button, etc.).
    public void OnPausePressed()
    {
        if (Session.CurrentState is PausedState)
            Session.Resume();
        else
            Session.RequestPause(PauseReason.Player);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;            // never leave the editor frozen
        AudioListener.pause = false;
        if (Session != null) Session.Registry.ShutdownAll();
        if (Instance == this) Instance = null;
    }

    // Quick test harness: remove or hide for real builds.
    void OnGUI()
    {
        if (!showDebugGui || Session == null) return;

        GUI.Label(new Rect(20, 20, 400, 25),
            Session.CurrentState.GetType().Name + "  |  features: " + Session.Registry.Count);

        if (GUI.Button(new Rect(20, 50, 160, 30), "Pause / Resume"))
            OnPausePressed();

        if (stressPrefab != null && GUI.Button(new Rect(20, 90, 160, 30), "Spawn 100"))
        {
            for (int i = 0; i < 100; i++)
                Instantiate(stressPrefab, Random.insideUnitCircle * 8f, Quaternion.identity);
        }
    }
}
