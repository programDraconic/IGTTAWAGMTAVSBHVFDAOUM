using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int hitsToKill = 3;
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private CrystalDropTable crystalDropTable;
    [SerializeField, Min(0f)] private float dropScatterRadius = 0.55f;

    [Header("Death Events")]
    [SerializeField] private UnityEvent onDeath = new UnityEvent();

    private int hitsRemaining;
    private bool isDead;

    private void Awake()
    {
        hitsRemaining = Mathf.Max(1, hitsToKill);
    }

    public void TakeHit()
    {
        if (isDead) return;

        hitsRemaining--;
        Debug.Log($"{name}: hit received; {hitsRemaining} hits remaining", this);

        if (hitsRemaining <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        // Notify listeners without interrupting the existing death logic.
        isDead = true;
        try
        {
            onDeath.Invoke();
        }
        catch (System.Exception exception)
        {
        Debug.LogException(exception, this);
        }
        Debug.Log($"{name}: killed; despawning", this);

        // Loot must never be able to prevent the enemy from dying.
        try
        {
            SpawnDrops();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }
        finally
        {
            Destroy(gameObject);
        }
    }

    private void SpawnDrops()
    {
        if (crystalDropTable != null)
        {
            if (!crystalDropTable.TryRoll(out CrystalPickup prefab, out int count))
                return; // A no-drop result is valid.

            for (int i = 0; i < count; i++)
            {
                Vector2 scatter = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
                Instantiate(prefab, transform.position + (Vector3)scatter, Quaternion.identity);
            }
        }
        else if (crystalPrefab != null)
        {
            Instantiate(crystalPrefab, transform.position, Quaternion.identity);
        }
    }
}
