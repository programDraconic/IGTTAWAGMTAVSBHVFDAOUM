// The only state where the simulation runs: advances run time and difficulty,
// and ticks every registered feature (weapons, enemies, summons...).
public class PlayingState : RunState
{
    public override bool HandlePause(RunSession session, PauseReason reason)
    {
        if (session == null) return false;

        RunState next = reason == PauseReason.LevelUp
            ? (RunState)new LevelUpPausedState()
            : new PausedState();
        session.TransitionTo(next);
        return true;
    }

    public override string Tick(RunSession session, FrameContext frame)
    {
        if (session != null && frame != null)
        {
            session.AdvanceRun(frame.DeltaTime);
            session.Registry.DispatchTick(frame);
        }
        return "PlayingState: simulation running";
    }
}
