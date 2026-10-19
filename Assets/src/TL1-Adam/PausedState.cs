// Player pause. Features are not ticked, so the game freezes for free.
public class PausedState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "PausedState: simulation frozen";
    }
}
