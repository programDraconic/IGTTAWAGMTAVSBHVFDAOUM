// Assets/src/TL5-Wyatt/SuitorView.cs
// The MonoBehaviour on the SuitorCard prefab. One instance per suitor, created at
// run time with Instantiate(). Draws that suitor's card and routes button presses
// back through DatingController's public surface.

using UnityEngine;

public class SuitorView : MonoBehaviour
{
    private Suitor suitor;
    private DatingController controller;
    private int slot;
    private string lastLine = "";
    private Mood lastMood = Mood.Neutral;

    public void Bind(Suitor boundSuitor, int slotIndex, DatingController owningController)
    {
        suitor = boundSuitor;
        slot = slotIndex;
        controller = owningController;
        gameObject.name = "SuitorCard - " + boundSuitor.DisplayName;
        transform.position = new Vector3(slotIndex * 2.5f, 0f, 0f);
    }

    private void Select()
    {
        controller.SelectSuitor(slot);
    }

    private void OnGUI()
    {
        if (suitor == null) return;

        float x = 20f + slot * 250f;
        float y = 70f;
        GUI.Box(new Rect(x, y, 240f, 250f), GUIContent.none);

        GUI.Label(new Rect(x + 10f, y + 8f, 220f, 22f), suitor.DisplayName);
        GUI.Label(new Rect(x + 10f, y + 30f, 220f, 22f),
                  "Stage: " + suitor.State.Stage + "   Affection: " + suitor.State.Affection);
        GUI.Label(new Rect(x + 10f, y + 50f, 220f, 22f),
                  "Next stage in: " + suitor.State.AffectionToNextStage());

        float by = y + 76f;
        if (GUI.Button(new Rect(x + 10f, by, 220f, 22f), "Begin Conversation"))
        {
            Select();
            Show(controller.BeginConversation());
        }
        by += 26f;

        foreach (Gift gift in Gift.Catalogue)
        {
            if (GUI.Button(new Rect(x + 10f, by, 220f, 22f), "Gift: " + gift.DisplayName))
            {
                Select();
                Show(controller.OfferGift(gift));
            }
            by += 24f;
        }

        if (GUI.Button(new Rect(x + 10f, by, 220f, 22f), "Advance Relationship"))
        {
            Select();
            bool moved = controller.Advance();
            lastLine = moved
                ? "-> advanced to " + suitor.State.Stage
                : "-> not enough affection yet";
            lastMood = moved ? Mood.Delighted : Mood.Annoyed;
        }

        GUI.Label(new Rect(x + 10f, y + 218f, 220f, 40f), "[" + lastMood + "] " + lastLine);
    }

    private void Show(Reaction reaction)
    {
        if (reaction == null) return;
        lastLine = reaction.Line;
        lastMood = reaction.Mood;
    }
}
