// Assets/src/TL5-Wyatt/Reaction.cs
// Return type of SelectReaction() on the TL5 class diagram.

public enum Mood { Insulted, Annoyed, Neutral, Pleased, Delighted }

public class Reaction
{
    public readonly string Line;
    public readonly Mood Mood;

    public Reaction(string line, Mood mood)
    {
        Line = line;
        Mood = mood;
    }

    public override string ToString() { return Line; }
}
