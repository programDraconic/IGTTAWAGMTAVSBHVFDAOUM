using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSummonController : MonoBehaviour
{
    [SerializeField] private EnemyPlaceholder target;

    private Summon[] summons;
    private int currentIndex;

    private void Start()
    {
        summons = new Summon[]
        {
            new OrbitingSummon(),
            new HuntingSummon(),
            new TotemSummon(),
            new RevengeSummon()
        };
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % summons.Length;
            Debug.Log("Selected: " + summons[currentIndex].GetType().Name);
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
            UseSummon();
    }

    private void UseSummon()
    {
        Summon summon = summons[currentIndex];
        summon.Activate();

        Vector2 targetPosition = target != null
            ? (Vector2)target.transform.position
            : Vector2.zero;

        string targetName = target != null ? target.name : "Enemy-01";
        bool hasTarget = target != null;

        SummonContext context =
            new SummonContext(targetPosition, targetName, hasTarget);

        summon.UpdateBehavior(context);
        Debug.Log(summon.LastBehavior);
    }
}