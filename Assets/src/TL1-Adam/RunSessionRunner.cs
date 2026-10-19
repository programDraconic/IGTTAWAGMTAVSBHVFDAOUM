using UnityEngine;

// The only MonoBehaviour: bridges Unity's Update loop to the plain-C# session.
public class RunSessionRunner : MonoBehaviour
{
    [SerializeField] private float countdownSeconds = 3f;

    public RunSession Session { get; private set; }

    void Awake()
    {
        Session = new RunSession();
        RunHandle handle = Session.Initialize(new RunConfig { CountdownSeconds = countdownSeconds });
        Session.StartRun(handle.RunId);
    }

    void Update()
    {
        Session.Tick(new FrameContext(Time.deltaTime, Time.time));
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
        if (Session != null) Session.Registry.ShutdownAll();
    }
}
