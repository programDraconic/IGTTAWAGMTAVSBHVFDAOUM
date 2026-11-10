using UnityEngine;
using UnityEngine.SceneManagement;

public class DatingSceneController : MonoBehaviour
{
    [SerializeField] private GameObject suitorCardPrefab;
    [SerializeField] private string returnSceneName = "S3-S7-GameplayLoop";
    [Range(0.4f, 3f)]
    [SerializeField] private float uiScale = 1f;

    private DatingFeature feature;
    private DatingController controller;
    private Suitor[] suitors;
    private SuitorView view;
    private int current;

    private void Start()
    {
        DatingUi.UserScale = uiScale;

        feature = GetComponent<DatingFeature>();
        if (feature == null) feature = FindObjectOfType<DatingFeature>();
        if (feature == null)
        {
            Debug.LogError("DatingSceneController: no DatingFeature in the scene.");
            enabled = false;
            return;
        }

        controller = feature.Controller;
        suitors = controller.GetSuitors();

        GameObject prefab = suitorCardPrefab != null
            ? suitorCardPrefab
            : Resources.Load<GameObject>("SuitorCard");

        if (prefab == null)
        {
            Debug.LogError("DatingSceneController: SuitorCard prefab not found in Resources.");
            enabled = false;
            return;
        }

        GameObject card = Instantiate(prefab, transform);
        view = card.GetComponent<SuitorView>();
        Show(0);
    }

    private void Show(int index)
    {
        if (suitors == null || suitors.Length == 0 || view == null) return;
        current = (index + suitors.Length) % suitors.Length;
        controller.SelectSuitor(current);
        view.Bind(suitors[current], current, controller);
    }

    private void Leave()
    {
        feature.Capture();
        if (Application.CanStreamedLevelBeLoaded(returnSceneName))
        {
            SceneManager.LoadScene(returnSceneName);
            return;
        }
        Debug.LogWarning("DatingSceneController: scene '" + returnSceneName +
                         "' is not in Build Settings. Add it under File > Build Settings.");
    }

    private void OnGUI()
    {
        if (suitors == null || suitors.Length == 0) return;

        float s = DatingUi.Scale;
        float m = DatingUi.Margin;
        float w = DatingUi.CardWidth;
        float x = (Screen.width - w) * 0.5f;

        GUI.Label(new Rect(m, 14f * s, Screen.width - m * 2f, 28f * s),
                  "Woo the shrimp. Between fights, what you give them is what you keep.",
                  DatingUi.Header);

        float by = DatingUi.CardTop - 38f * s;
        if (GUI.Button(new Rect(x, by, 110f * s, 30f * s), "< Prev", DatingUi.Button)) Show(current - 1);
        GUI.Label(new Rect(x + 120f * s, by + 4f * s, w - 240f * s, 26f * s),
                  "Suitor " + (current + 1) + " of " + suitors.Length, DatingUi.Body);
        if (GUI.Button(new Rect(x + w - 110f * s, by, 110f * s, 30f * s), "Next >", DatingUi.Button)) Show(current + 1);

        float y = DatingUi.CardTop + DatingUi.CardHeight + 16f * s;
        GUI.Label(new Rect(m, y, Screen.width - m * 2f, 26f * s),
                  "Unlocks earned this visit: " + feature.UnlocksThisVisit +
                  "    Time in the hub: " + feature.PhaseSeconds.ToString("F0") + "s", DatingUi.Body);

        if (GUI.Button(new Rect(x, y + 34f * s, w, 34f * s), "Back to the fight", DatingUi.Button)) Leave();
    }
}
