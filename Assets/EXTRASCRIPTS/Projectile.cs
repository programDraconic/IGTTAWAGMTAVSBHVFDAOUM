using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float lifetime = 3f;

    private Rigidbody2D rb;
    private Transform ownerRoot;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(
        Vector2 direction,
        float speed,
        Transform owner)
    {
        ownerRoot = owner;
        hasHit = false;

        direction.Normalize();

        rb.gravityScale = 0f;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        rb.rotation = angle;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        if (ownerRoot != null &&
            (other.transform == ownerRoot ||
             other.transform.IsChildOf(ownerRoot)))
        {
            return;
        }
        // Let projectiles pass through crystal pickups.
        if (other.GetComponentInParent<CrystalPickup>() != null)
        {
            return;
        }
        hasHit = true;

        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            Debug.Log(
                $"Projectile hit enemy: {enemy.name}",
                enemy
            );

            enemy.TakeHit();
        }
        else
        {
            Debug.Log(
                $"Projectile hit: {other.name}. " +
                "No matching EnemyHealth component found.",
                other
            );
        }

        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}