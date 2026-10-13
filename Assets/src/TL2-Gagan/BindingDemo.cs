using UnityEngine;

// Add this component to an empty GameObject in the TL2-Gagan demo scene.
public class BindingDemo : MonoBehaviour
{
    private Summon currentSummon;
    private int currentIndex;

    private readonly string[] summonNames =
    {
        "OrbitingSummon",
        "HuntingSummon",
        "TotemSummon",
        "RevengeSummon"
    };

    private void Start()
    {
        SelectSummon(0);
    }

    private void SelectSummon(int index)
    {
        currentIndex = index;

        switch (currentIndex)
        {
            case 0:
                currentSummon = new OrbitingSummon();
                break;
            case 1:
                currentSummon = new HuntingSummon();
                break;
            case 2:
                currentSummon = new TotemSummon();
                break;
            default:
                currentSummon = new RevengeSummon();
                break;
        }

        currentSummon.Activate();

        SummonContext context = new SummonContext(
            Vector2.zero,
            "Enemy-01",
            true);

        // The variable type is Summon, but the actual subclass override runs.
        currentSummon.UpdateBehavior(context);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 30),
            "Base reference type: Summon");

        GUI.Label(new Rect(20, 50, 500, 30),
            "Actual object: " + summonNames[currentIndex]);

        GUI.Label(new Rect(20, 80, 600, 30),
            "Override result: " + currentSummon.LastBehavior);

        if (GUI.Button(new Rect(20, 125, 180, 35), "Next Summon"))
        {
            int nextIndex = (currentIndex + 1) % summonNames.Length;
            SelectSummon(nextIndex);
        }
    }
}
