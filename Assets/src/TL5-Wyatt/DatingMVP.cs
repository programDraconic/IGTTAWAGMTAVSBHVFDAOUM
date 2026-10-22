using System.Collections.Generic;
using UnityEngine;

public class DatingMVP : MonoBehaviour
{
    [Tooltip("Assets/prefabs/TL5-Wyatt/Resources/SuitorCard.prefab. " +
             "Left empty, it is loaded from Resources at run time.")]
    public GameObject suitorCardPrefab;

    [Tooltip("Card size multiplier. The card already scales with the Game view " +
             "width; raise this if it is still too small. Also adjustable in Play mode.")]
    [Range(0.4f, 3f)]
    public float uiScale = 1f;

    private DatingController controller;
    private Suitor[] suitors;
    private GameObject card;
    private SuitorView view;
    private int current;
    private readonly List<string> unlockLog = new List<string>();

    private void Start()
    {
        DatingUi.UserScale = uiScale;

        controller = new DatingController();
        controller.UnlockEarned += OnUnlockEarned;
        controller.EnterDatingPhase();
        suitors = controller.GetSuitors();

        GameObject prefab = suitorCardPrefab != null
            ? suitorCardPrefab
            : Resources.Load<GameObject>("SuitorCard");

        if (prefab == null)
        {
            Debug.LogError("DatingMVP: SuitorCard prefab not found. Assign it in the Inspector " +
                           "or keep it in Assets/prefabs/TL5-Wyatt/Resources/.");
            return;
        }

        card = Instantiate(prefab, transform);
        view = card.GetComponent<SuitorView>();
        ShowSuitor(0);
    }

    private void ShowSuitor(int index)
    {
        if (suitors == null || suitors.Length == 0 || view == null) return;
        current = (index + suitors.Length) % suitors.Length;
        controller.SelectSuitor(current);
        view.Bind(suitors[current], current, controller);
    }

    private void OnUnlockEarned(RelationshipUnlock unlock)
    {
        unlockLog.Add(unlock.ToString());
        Debug.Log("[F6 -> F3] " + unlock);
    }

    private void OnGUI()
    {
        if (suitors == null || suitors.Length == 0) return;

        float s = DatingUi.Scale;
        float m = DatingUi.Margin;
        float w = DatingUi.CardWidth;
        float x = (Screen.width - w) * 0.5f;

        GUI.Label(new Rect(m, 14f * s, Screen.width - m * 2f, 28f * s),
                  "TL5 / F6 - Dating System MVP   |   one SuitorCard prefab instance, rebound per suitor",
                  DatingUi.Header);

        float by = DatingUi.CardTop - 38f * s;
        if (GUI.Button(new Rect(x, by, 110f * s, 30f * s), "< Prev", DatingUi.Button))
            ShowSuitor(current - 1);
        GUI.Label(new Rect(x + 120f * s, by + 4f * s, w - 240f * s, 26f * s),
                  "Suitor " + (current + 1) + " of " + suitors.Length, DatingUi.Body);
        if (GUI.Button(new Rect(x + w - 110f * s, by, 110f * s, 30f * s), "Next >", DatingUi.Button))
            ShowSuitor(current + 1);

        float bx = Screen.width - m - 150f * s;
        GUI.Label(new Rect(bx, 14f * s, 60f * s, 28f * s), "Size", DatingUi.Body);
        if (GUI.Button(new Rect(bx + 55f * s, 14f * s, 42f * s, 28f * s), "A-", DatingUi.Button))
            DatingUi.UserScale = Mathf.Clamp(DatingUi.UserScale - 0.15f, 0.4f, 3f);
        if (GUI.Button(new Rect(bx + 102f * s, 14f * s, 42f * s, 28f * s), "A+", DatingUi.Button))
            DatingUi.UserScale = Mathf.Clamp(DatingUi.UserScale + 0.15f, 0.4f, 3f);

        float y = DatingUi.CardTop + DatingUi.CardHeight + 16f * s;
        GUI.Label(new Rect(m, y, Screen.width - m * 2f, 26f * s),
                  "Unlock events published to F3:", DatingUi.Header);
        for (int i = 0; i < unlockLog.Count; i++)
            GUI.Label(new Rect(m + 16f * s, y + 28f * s + i * 24f * s,
                               Screen.width - m * 2f, 24f * s),
                      "- " + unlockLog[i], DatingUi.Body);
    }

    private void OnDestroy()
    {
        if (controller != null) controller.UnlockEarned -= OnUnlockEarned;
    }
}
