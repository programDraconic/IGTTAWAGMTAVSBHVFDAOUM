
using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    [SerializeField] private int crystalValue = 1;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCrystals player =
            other.GetComponentInParent<PlayerCrystals>();

        if (player == null || collected)
            return;

        collected = true;

        player.AddCrystals(crystalValue);

        Destroy(gameObject);
    }
}
