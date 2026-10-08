
public class PlayingState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "PlayingState: Overrode default RunState.";
    }
}