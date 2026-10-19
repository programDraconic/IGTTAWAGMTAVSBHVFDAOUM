// What a state or feature needs to know about the current frame.
// A plain class (not a MonoBehaviour) so tests can call `new FrameContext()`.
public class FrameContext
{
    public float DeltaTime { get; }
    public float Time { get; }

    public FrameContext(float deltaTime = 0f, float time = 0f)
    {
        DeltaTime = deltaTime;
        Time = time;
    }
}
