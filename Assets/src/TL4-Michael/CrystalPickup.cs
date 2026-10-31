using UnityEngine;

// Attach to each crystal prefab. Set the crystal and XP rewards in its Inspector.
public class CrystalPickup : MonoBehaviour
{
    [SerializeField, Min(0)] private int crystalValue = 1;
    [SerializeField, Min(0)] private int xpValue = 25;
    private bool collected;
    private Rigidbody2D body;

    public bool IsCollected => collected;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCrystals player = other.GetComponentInParent<PlayerCrystals>();
        if (player != null) Collect(player);
    }

    public void PullTowards(Vector2 target, float speed, float step)
    {
        if (collected) return;
        Vector2 next = Vector2.MoveTowards(transform.position, target, speed * step);
        if (body != null) body.MovePosition(next);
        else transform.position = next;
    }

    // Magnet can collect directly when a crystal reaches the player,
    // even if its collider arrangement doesn't produce a trigger callback.
    public void Collect(PlayerCrystals player)
    {
        if (collected || player == null) return;
        collected = true;
        player.AddCrystals(crystalValue);
        PlayerStats.Shared.GainXP(xpValue);
        Destroy(gameObject);
    }
}
