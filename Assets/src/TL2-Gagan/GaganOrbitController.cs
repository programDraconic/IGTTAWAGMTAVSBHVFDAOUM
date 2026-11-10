using UnityEngine;

public class GaganOrbitController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Min(0f)] private float radius = 2f;
    [SerializeField] private float degreesPerSecond = 90f;

    private OrbitingSummon orbitingSummon;
    private Summon activeSummon;

    private void Awake()
    {
        orbitingSummon = new OrbitingSummon(
            radius,
            degreesPerSecond);

        // Base reference calls the subclass override.
        activeSummon = orbitingSummon;
        activeSummon.Activate();
    }

    private void Update()
    {
        if (player == null || !activeSummon.Active ||
            Time.deltaTime <= 0f)
        {
            return;
        }

        SummonContext context = new SummonContext(
            player.position,
            deltaTime: Time.deltaTime);

        activeSummon.UpdateBehavior(context);

        Vector2 position = orbitingSummon.Position;

        transform.position = new Vector3(
            position.x,
            position.y,
            transform.position.z);
    }

    private void OnDestroy()
    {
        if (activeSummon != null)
            activeSummon.Deactivate();
    }
}
