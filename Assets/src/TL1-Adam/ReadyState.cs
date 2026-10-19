// Pre-run countdown. Moves to PlayingState when it hits zero.
public class ReadyState : RunState
{
    private float remaining;

    public override void EnterState(RunSession session)
    {
        remaining = session != null ? session.Config.CountdownSeconds : 0f;
    }

    public override string Tick(RunSession session, FrameContext frame)
    {
        if (session == null || !session.IsRunActive)
            return "ReadyState: waiting for StartRun";

        remaining -= frame != null ? frame.DeltaTime : 0f;
        if (remaining <= 0f)
        {
            session.TransitionTo(new PlayingState());
            return "ReadyState: countdown finished";
        }
        return "ReadyState: counting down";
    }
}
