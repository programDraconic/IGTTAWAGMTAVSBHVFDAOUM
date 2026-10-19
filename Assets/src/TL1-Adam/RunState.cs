// The abstract State. Tick is the virtual method the binding tests exercise.
// Tick returns a string for now so tests have something to compare
// (the binding assignment allows this); the diagram says void.
public abstract class RunState
{
    public virtual void EnterState(RunSession session) { }

    // Default: this state cannot be paused. PlayingState overrides it.
    public virtual bool HandlePause(RunSession session, PauseReason reason)
    {
        return false;
    }

    public virtual string Tick(RunSession session, FrameContext frame)
    {
        return "RunState: no state defined, nothing to do";
    }
}
