
public abstract class RunState
{
    public virtual string Tick(RunSession session, FrameContext frame)
    {
        return "RunState: no state defined, nothing to do";
    }
}