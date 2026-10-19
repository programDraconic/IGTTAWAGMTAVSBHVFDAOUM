using System;

// Small supporting types referenced by the class diagram.
// Plain C# on purpose: tests can create them with `new`.

public enum PauseReason { Player, LevelUp, System }

public enum EndReason { PlayerDied, Victory, Quit }

public class RunConfig
{
    public float CountdownSeconds = 3f;      // Ready -> Playing countdown
    public float BaseDifficulty = 1f;        // starting difficulty scalar
    public float DifficultyPerSecond = 0.01f; // how fast it ramps while Playing
}

public readonly struct RunHandle
{
    public readonly Guid RunId;
    public RunHandle(Guid runId) { RunId = runId; }
}

public readonly struct RunResult
{
    public readonly EndReason Reason;
    public readonly float Duration;
    public readonly float FinalDifficulty;

    public RunResult(EndReason reason, float duration, float finalDifficulty)
    {
        Reason = reason;
        Duration = duration;
        FinalDifficulty = finalDifficulty;
    }
}
