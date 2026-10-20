// Assets/src/TL5-Wyatt/DatingMVP.cs
// TL5 MVP driver. Builds the dating phase and INSTANTIATES one SuitorCard prefab
// per suitor at run time. Put this on one empty GameObject in your own scene.

using System.Collections.Generic;
using UnityEngine;

public class DatingMVP : MonoBehaviour
{
    [Tooltip("Assets/prefabs/TL5-Wyatt/Resources/SuitorCard.prefab. " +
             "Left empty, it is loaded from Resources at run time.")]
    public GameObject suitorCardPrefab;

    private DatingController controller;
    private readonly List<string> unlockLog = new List<string>();
    private readonly List<GameObject> spawned = new List<GameObject>();

    private void Start()
    {
        controller = new DatingController();
        controller.UnlockEarned += OnUnlockEarned;   // F3 subscribes here in the real build
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
        GUI.Label(new Rect(20f, 10f, 900f, 22f),
                  "TL5 / F6 - Dating System MVP   |   " + spawned.Count + " suitor cards instantiated from prefab");
        GUI.Label(new Rect(20f, 32f, 900f, 22f),
                  "Gift, then Advance Relationship. Each suitor values gifts differently - that is the dynamic binding.");

        float y = 330f;
        GUI.Label(new Rect(20f, y, 900f, 22f), "Unlock events published to F3:");
        for (int i = 0; i < unlockLog.Count; i++)
            GUI.Label(new Rect(36f, y + 20f + i * 18f, 900f, 20f), "- " + unlockLog[i]);
    }

    private void OnDestroy()
    {
        if (controller != null) controller.UnlockEarned -= OnUnlockEarned;
    }
}
