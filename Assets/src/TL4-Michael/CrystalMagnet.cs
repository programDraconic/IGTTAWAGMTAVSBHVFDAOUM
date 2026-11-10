using UnityEngine;

// Attach to the player GameObject that has PlayerCrystals.
// Detects crystal pickups in range and pulls them to the player.
[RequireComponent(typeof(PlayerCrystals))]
public class CrystalMagnet : MonoBehaviour
{
    [SerializeField, Min(0f)] private float magnetRadius = 4f;
    [SerializeField, Min(0f)] private float pullSpeed = 9f;
    [SerializeField, Min(0f)] private float pickupDistance = 0.35f;
    [SerializeField] private LayerMask crystalLayers = ~0;

    private PlayerCrystals player;

    public float MagnetRadius
    {
        get => magnetRadius;
        set => magnetRadius = Mathf.Max(0f, value);
    }

    public float PullSpeed
    {
        get => pullSpeed;
        set => pullSpeed = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        player = GetComponent<PlayerCrystals>();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f || magnetRadius <= 0f) return;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, magnetRadius, crystalLayers);
        foreach (Collider2D hit in nearby)
        {
            CrystalPickup crystal = hit.GetComponentInParent<CrystalPickup>();
            if (crystal == null || crystal.IsCollected) continue;

            // Check distance to the crystal itself, not a child collider.
            Vector2 target = transform.position;
            if (Vector2.Distance(crystal.transform.position, target) <= pickupDistance)
            {
                crystal.Collect(player);
                continue;
            }

            crystal.PullTowards(target, pullSpeed, Time.fixedDeltaTime);
            if (Vector2.Distance(crystal.transform.position, target) <= pickupDistance)
                crystal.Collect(player);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
