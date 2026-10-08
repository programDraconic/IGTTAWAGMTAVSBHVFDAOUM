public class LevelUpPausedState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "LevelUpPausedState: Overrode default RunState.";
    }
}