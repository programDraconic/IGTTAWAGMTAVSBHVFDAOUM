using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    [SerializeField] private int crystalValue = 1;
    [SerializeField] private int xpValue = 25;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCrystals player = other.GetComponentInParent<PlayerCrystals>();

        if (player == null || collected)
            return;

        collected = true;

        // Keep the existing crystal currency pickup behavior.
        player.AddCrystals(crystalValue);

        // The same shared XP pool used by the card level-up system.
        PlayerStats.Shared.GainXP(xpValue);

        Destroy(gameObject);
    }
}
