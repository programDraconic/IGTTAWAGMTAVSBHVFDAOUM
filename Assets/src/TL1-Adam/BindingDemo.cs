using UnityEngine;

public class BindingDemo : MonoBehaviour
{
    RunState current = new PlayingState(); // Declared: RunState. Actual = PlayingState.
    
    RunSession session = new RunSession();
    FrameContext frame = new FrameContext();

    void OnGUI()
    {
        GUI.Label(new Rect(20,20,400,30), current.Tick(session, frame));
        if (GUI.Button(new Rect(20,60,160,30), "Swap"))
            current = (current is PlayingState) ? new BaseOnlyState() : new PlayingState();
    }
}
