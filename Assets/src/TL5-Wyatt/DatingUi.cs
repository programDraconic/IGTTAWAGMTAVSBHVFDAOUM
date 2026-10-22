// Assets/src/TL5-Wyatt/DatingUi.cs
// Shared sizing and styles for the MVP's on-screen cards. Scales with the
// Game view width so the cards stay readable at any resolution.

using UnityEngine;

public static class DatingUi
{
    // Multiplier on top of the automatic scale. Driven by the A+ / A- buttons
    // and by the UI Scale field on DatingMVP.
    public static float UserScale = 1f;

    public static float Scale
    {
        get
        {
            float auto = Screen.width / 1600f;
            return Mathf.Clamp(auto, 0.45f, 3f) * Mathf.Clamp(UserScale, 0.4f, 3f);
        }
    }

    public static float Margin      { get { return 24f * Scale; } }
    public static float CardWidth   { get { return 360f * Scale; } }
    public static float CardSpacing { get { return 380f * Scale; } }
    public static float CardHeight  { get { return 430f * Scale; } }
    public static float CardTop     { get { return 96f * Scale; } }
    public static float Row         { get { return 36f * Scale; } }

    private static int cachedSize = -1;
    private static GUIStyle title, body, button, header;

    private static void Rebuild()
    {
        int fs = Mathf.Max(9, Mathf.RoundToInt(17f * Scale));
        if (fs == cachedSize && title != null) return;
        cachedSize = fs;

        title = new GUIStyle(GUI.skin.label);
        title.fontSize = fs + 5;
        title.fontStyle = FontStyle.Bold;
        title.wordWrap = true;

        body = new GUIStyle(GUI.skin.label);
        body.fontSize = fs;
        body.wordWrap = true;

        button = new GUIStyle(GUI.skin.button);
        button.fontSize = fs;

        header = new GUIStyle(GUI.skin.label);
        header.fontSize = fs + 2;
        header.wordWrap = true;
    }

    public static GUIStyle Title  { get { Rebuild(); return title; } }
    public static GUIStyle Body   { get { Rebuild(); return body; } }
    public static GUIStyle Button { get { Rebuild(); return button; } }
    public static GUIStyle Header { get { Rebuild(); return header; } }
}
