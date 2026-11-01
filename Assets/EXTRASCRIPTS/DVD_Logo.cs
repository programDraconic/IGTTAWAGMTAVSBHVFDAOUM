using UnityEngine;

public class DVD_Logo : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float spinSpeed = 720f; // Degrees per second

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the Rigidbody doesn't slow down from linear drag
        if (rb != null)
        {
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }

        // Give it a random diagonal starting direction (like the DVD logo)
        float randomX = Random.value > 0.5f ? 1f : -1f;
        float randomY = Random.value > 0.5f ? 1f : -1f;
        moveDirection = new Vector2(randomX, randomY).normalized;
    }

    private void Update()
    {
        // Spin the object continuously around its Z-axis
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // Keep moving at a constant velocity
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When it hits a wall or collider, reflect the direction vector off the surface normal
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
        }
    }
}