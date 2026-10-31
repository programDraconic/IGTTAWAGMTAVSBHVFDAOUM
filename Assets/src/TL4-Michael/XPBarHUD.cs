using UnityEngine;

// Displays the shared player's current level and XP progress.
// Attach to an active GameObject (such as a HUD manager).
// Uses legacy Unity OnGUI: no Canvas, TextMeshPro or Input System required.
public class XPBarHUD : MonoBehaviour
{
    [SerializeField] private float barWidth = 360f;
    [SerializeField] private float barHeight = 24f;
    [SerializeField] private float bottomPadding = 24f;
    [SerializeField] private Color fillColor = new Color(0.25f, 0.7f, 1f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0.08f, 0.1f, 0.16f, 0.9f);

    private GUIStyle centeredText;

    private void OnGUI()
    {
        PlayerStats stats = PlayerStats.Shared;
        int required = Mathf.Max(1, stats.XPToNextLevel);
        float progress = Mathf.Clamp01((float)stats.XP / required);

        float width = Mathf.Min(barWidth, Screen.width - 24f);
        float x = (Screen.width - width) * 0.5f;
        float y = Screen.height - bottomPadding - barHeight;
        Rect bar = new Rect(x, y, width, barHeight);

        // A background, then a filled portion proportional to XP progress.
        Color oldColor = GUI.color;
        GUI.color = backgroundColor;
        GUI.DrawTexture(bar, Texture2D.whiteTexture);
        GUI.color = fillColor;
        GUI.DrawTexture(new Rect(x + 2f, y + 2f,
            Mathf.Max(0f, (width - 4f) * progress),
            Mathf.Max(0f, barHeight - 4f)), Texture2D.whiteTexture);
        GUI.color = oldColor;

        if (centeredText == null)
        {
            centeredText = new GUIStyle(GUI.skin.label);
            centeredText.alignment = TextAnchor.MiddleCenter;
            centeredText.fontStyle = FontStyle.Bold;
            centeredText.normal.textColor = Color.white;
        }
        GUI.Label(bar, "Level " + stats.Level + "    " + stats.XP + " / " + required + " XP", centeredText);
    }
}
