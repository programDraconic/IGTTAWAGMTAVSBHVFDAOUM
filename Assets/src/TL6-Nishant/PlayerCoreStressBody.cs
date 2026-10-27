using UnityEngine;

namespace TL6
{
    // A small F7 test body, not the completed player interface in the Champion.
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class PlayerCoreStressBody : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 2f;
        [SerializeField, Min(0f)] private float baseMight = 10f;

        private Rigidbody2D body;
        private Vector2 direction = Vector2.right;
        private Vector2 arenaHalfSize = new Vector2(8f, 4f);
        private StatModifier modifier = new PercentModifier();

        public float Might { get; private set; } = 12f;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Initialise(Vector2 heading, Vector2 halfSize)
        {
            body = GetComponent<Rigidbody2D>();
            direction = heading.sqrMagnitude > 0f ? heading.normalized : Vector2.right;
            arenaHalfSize = new Vector2(Mathf.Max(0.5f, halfSize.x), Mathf.Max(0.5f, halfSize.y));
            UsePercentModifier(true);
        }

        public void UsePercentModifier(bool usePercent)
        {
            modifier = usePercent ? (StatModifier)new PercentModifier() : new FlatModifier();
            Might = modifier.Apply(baseMight);
        }

        private void FixedUpdate()
        {
            Vector2 position = body.position;
            if (position.x >= arenaHalfSize.x) direction.x = -Mathf.Abs(direction.x);
            if (position.x <= -arenaHalfSize.x) direction.x = Mathf.Abs(direction.x);
            if (position.y >= arenaHalfSize.y) direction.y = -Mathf.Abs(direction.y);
            if (position.y <= -arenaHalfSize.y) direction.y = Mathf.Abs(direction.y);

            body.linearVelocity = direction * moveSpeed;
            // Calls the existing override through a StatModifier base reference.
            Might = modifier.Apply(baseMight);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.contactCount > 0)
                direction = Vector2.Reflect(direction, collision.GetContact(0).normal).normalized;
        }
    }
}
