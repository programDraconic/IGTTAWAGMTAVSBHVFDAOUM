using System;

// The context: owns the current state and the feature registry, and forwards
// work to the current state. This is where dynamic binding pays off.
public class RunSession
{
    private RunState currentState;
    private RunState resumeState;                 // where Resume() returns to
    private readonly FeatureRegistry registry;
    private RunConfig config = new RunConfig();

    // Phase events other teams subscribe to (Diagram 0: "phase events").
    public event Action<RunState> PhaseChanged;

    public RunState CurrentState { get { return currentState; } }
    public FeatureRegistry Registry { get { return registry; } }
    public RunConfig Config { get { return config; } }

    public Guid RunId { get; private set; }
    public bool IsRunActive { get; private set; }
    public float ElapsedTime { get; private set; }
    public float DifficultyScalar { get; private set; }   // read by 4.0 Enemies

    public RunSession()
    {
        registry = new FeatureRegistry(this);
        DifficultyScalar = config.BaseDifficulty;
        TransitionTo(new ReadyState());
    }

    public RunHandle Initialize(RunConfig newConfig)
    {
        config = newConfig ?? new RunConfig();
        Reset();
        return new RunHandle(Guid.NewGuid());
    }

    public bool StartRun(Guid runId)
    {
        if (IsRunActive) return false;

        RunId = runId;
        IsRunActive = true;
        ElapsedTime = 0f;
        DifficultyScalar = config.BaseDifficulty;
        resumeState = null;
        TransitionTo(new ReadyState());
        return true;
    }

    // Called once per frame by the runner. Which Tick runs depends on the
    // actual object in currentState, not its declared type: dynamic binding.
    public void Tick(FrameContext frame)
    {
        currentState.Tick(this, frame);
    }

    public bool RequestPause(PauseReason reason)
    {
        RunState before = currentState;
        if (!currentState.HandlePause(this, reason)) return false;

        resumeState = before;
        return true;
    }

    public bool Resume()
    {
        if (resumeState == null) return false;

        RunState target = resumeState;
        resumeState = null;
        TransitionTo(target);
        return true;
    }

    public RunResult EndRun(EndReason reason)
    {
        RunResult result = new RunResult(reason, ElapsedTime, DifficultyScalar);
        IsRunActive = false;
        resumeState = null;
        TransitionTo(new ReadyState());
        return result;
    }

    public void Reset()
    {
        IsRunActive = false;
        ElapsedTime = 0f;
        DifficultyScalar = config.BaseDifficulty;
        resumeState = null;
        TransitionTo(new ReadyState());
    }

    public void TransitionTo(RunState newState)
    {
        if (newState == null) throw new ArgumentNullException("newState");

        currentState = newState;
        newState.EnterState(this);
        if (PhaseChanged != null) PhaseChanged(newState);
    }

    // Used by PlayingState each frame.
    internal void AdvanceRun(float deltaTime)
    {
        ElapsedTime += deltaTime;
        DifficultyScalar = config.BaseDifficulty + ElapsedTime * config.DifficultyPerSecond;
    }
}
