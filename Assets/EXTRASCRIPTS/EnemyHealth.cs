
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hitsToKill = 3;
    [SerializeField] private GameObject crystalPrefab;

    private int hitsRemaining;
    private bool isDead = false;

    private void Awake()
    {
        hitsRemaining = hitsToKill;
    }

    public void TakeHit()
    {
        if (isDead)
            return;

        hitsRemaining--;

        Debug.Log($"Enemy hit! {hitsRemaining} hits remaining.");

        if (hitsRemaining <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        if (crystalPrefab != null)
        {
            Instantiate(
                crystalPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
