using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class GiftStressBody : MonoBehaviour
{
    private static Sprite shared;

    private Rigidbody2D body;
    private DatingStressTest owner;
    private Vector2 arenaHalfSize = new Vector2(8f, 4.5f);
    private float escapeMargin = 1.5f;
    private int suitorIndex;
    private float speed = 6f;
    private bool reported;

    public static Sprite SharedSprite
    {
        get
        {
            if (shared == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                shared = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height),
                                       new Vector2(0.5f, 0.5f), tex.width / 0.24f);
            }
            return shared;
        }
    }

    public void Initialise(DatingStressTest test, int suitor, Vector2 halfSize, Vector2 heading)
    {
        owner = test;
        suitorIndex = suitor;
        arenaHalfSize = halfSize;
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.linearVelocity = (heading.sqrMagnitude > 0f ? heading.normalized : Vector2.right) * speed;
    }

    private void FixedUpdate()
    {
        if (body == null || reported) return;

        Vector2 p = body.position;

        if (Mathf.Abs(p.x) > arenaHalfSize.x + escapeMargin ||
            Mathf.Abs(p.y) > arenaHalfSize.y + escapeMargin)
        {
            reported = true;
            owner.ReportPhysicsViolation(name, p);
            return;
        }

        if (body.linearVelocity.sqrMagnitude < 0.01f)
            body.linearVelocity = Random.insideUnitCircle.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (owner != null) owner.ApplyGift(suitorIndex);
    }
}
