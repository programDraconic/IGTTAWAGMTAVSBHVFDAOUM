// Frozen while the player picks an upgrade (5.0 Experience & Draft).
// Whoever handles the choice calls session.Resume() afterward.
public class LevelUpPausedState : RunState
{
    public override string Tick(RunSession session, FrameContext frame)
    {
        return "LevelUpPausedState: waiting for upgrade choice";
    }
}
