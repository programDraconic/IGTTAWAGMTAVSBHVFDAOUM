using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{
    private PlayerStats stats = new PlayerStats();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("XP"))
            return;

        Pickup pickup = new XPPickup();
        string result = pickup.Collect(stats);

        Debug.Log($"{result} | XP: {stats.XP}");
        Destroy(other.gameObject);
    }
}