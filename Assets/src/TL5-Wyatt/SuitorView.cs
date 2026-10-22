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

        float s = DatingUi.Scale;
        float pad = 12f * s;
        float x = DatingUi.Margin + slot * DatingUi.CardSpacing;
        float y = DatingUi.CardTop;
        float w = DatingUi.CardWidth;
        float inner = w - pad * 2f;

        GUI.Box(new Rect(x, y, w, DatingUi.CardHeight), GUIContent.none);

        float cy = y + pad;
        GUI.Label(new Rect(x + pad, cy, inner, 30f * s), suitor.DisplayName, DatingUi.Title);
        cy += 34f * s;

        GUI.Label(new Rect(x + pad, cy, inner, 24f * s),
                  "Stage: " + suitor.State.Stage, DatingUi.Body);
        cy += 24f * s;
        GUI.Label(new Rect(x + pad, cy, inner, 24f * s),
                  "Affection: " + suitor.State.Affection +
                  "   (next stage in " + suitor.State.AffectionToNextStage() + ")", DatingUi.Body);
        cy += 32f * s;

        if (GUI.Button(new Rect(x + pad, cy, inner, 30f * s), "Begin Conversation", DatingUi.Button))
        {
            Select();
            Show(controller.BeginConversation());
        }
        cy += DatingUi.Row;

        foreach (Gift gift in Gift.Catalogue)
        {
            if (GUI.Button(new Rect(x + pad, cy, inner, 30f * s), "Gift: " + gift.DisplayName, DatingUi.Button))
            {
                Select();
                Show(controller.OfferGift(gift));
            }
            cy += DatingUi.Row;
        }

        if (GUI.Button(new Rect(x + pad, cy, inner, 30f * s), "Advance Relationship", DatingUi.Button))
        {
            Select();
            bool moved = controller.Advance();
            lastLine = moved
                ? "-> advanced to " + suitor.State.Stage
                : "-> not enough affection yet";
            lastMood = moved ? Mood.Delighted : Mood.Annoyed;
        }
        cy += DatingUi.Row + 6f * s;

        GUI.Label(new Rect(x + pad, cy, inner, DatingUi.CardHeight - (cy - y) - pad),
                  "[" + lastMood + "]  " + lastLine, DatingUi.Body);
    }

    private void Show(Reaction reaction)
    {
        if (reaction == null) return;
        lastLine = reaction.Line;
        lastMood = reaction.Mood;
    }
}
