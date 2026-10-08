public class ReadyState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "ReadyState: Overrode default RunState.";
    }
}