using System.Collections.Generic;
using UnityEngine;

public class DatingMVP : MonoBehaviour
{
    [Tooltip("Assets/prefabs/TL5-Wyatt/Resources/SuitorCard.prefab. " +
             "Left empty, it is loaded from Resources at run time.")]
    public GameObject suitorCardPrefab;

    [Tooltip("Card size multiplier. Cards already scale with the Game view width; " +
             "raise this if they are still too small. Also adjustable in Play mode.")]
    [Range(0.4f, 3f)]
    public float uiScale = 1f;

    private DatingController controller;
    private readonly List<string> unlockLog = new List<string>();
    private readonly List<GameObject> spawned = new List<GameObject>();

    private void Start()
    {
        DatingUi.UserScale = uiScale;

        controller = new DatingController();
        controller.UnlockEarned += OnUnlockEarned;
        controller.EnterDatingPhase();

        GameObject prefab = suitorCardPrefab != null
            ? suitorCardPrefab
            : Resources.Load<GameObject>("SuitorCard");

        if (prefab == null)
        {
            Debug.LogError("DatingMVP: SuitorCard prefab not found. Assign it in the Inspector " +
                           "or keep it in Assets/prefabs/TL5-Wyatt/Resources/.");
            return;
        }

        Suitor[] suitors = controller.GetSuitors();
        for (int i = 0; i < suitors.Length; i++)
        {
            GameObject card = Instantiate(prefab, transform);
            card.GetComponent<SuitorView>().Bind(suitors[i], i, controller);
            spawned.Add(card);
        }
    }

    private void OnUnlockEarned(RelationshipUnlock unlock)
    {
        unlockLog.Add(unlock.ToString());
        Debug.Log("[F6 -> F3] " + unlock);
    }

    private void OnGUI()
    {
        float s = DatingUi.Scale;
        float m = DatingUi.Margin;

        GUI.Label(new Rect(m, 14f * s, Screen.width - m * 2f, 28f * s),
                  "TL5 / F6 - Dating System MVP   |   " + spawned.Count +
                  " suitor cards instantiated from SuitorCard.prefab", DatingUi.Header);
        GUI.Label(new Rect(m, 44f * s, Screen.width - m * 2f - 200f * s, 28f * s),
                  "Gift a suitor, then Advance Relationship. The same gift lands differently on each " +
                  "- that is the dynamic binding.", DatingUi.Body);

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
