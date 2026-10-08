
public class PausedState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "PausedState: Overrode default RunState.";
    }
}
