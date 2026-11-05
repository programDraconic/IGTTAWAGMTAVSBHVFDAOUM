// Created by ChatGPT using the existing EnemyController code as a base.

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField, Min(0f)] private float attackRange = 1f;
    [SerializeField, Min(0.01f)] private float attackCooldown = 1f;
    [SerializeField, Min(1)] private int attackDamage = 5;

    // Keep the last facing direction when the player is almost directly above or below.
    private const float FacingDeadZone = 0.05f;

    private Rigidbody2D rb;
    private SpriteRenderer enemySprite;
    private EnemyState currentState;
    private float nextAttackTime = float.NegativeInfinity;

    public Transform Target => target;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public int AttackDamage => attackDamage;

    public bool IsDead { get; private set; }

    public bool HasTarget =>
        target != null && target.gameObject.activeInHierarchy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();

        // Let the enemy move freely in 2D without falling or spinning.
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        // Reset the enemy when it is enabled.
        IsDead = false;
        nextAttackTime = float.NegativeInfinity;

        ChangeState(new EnemyChaseState(this));
    }

    private void Update()
    {
        currentState?.Tick();
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        // Run the current state's physics logic on the physics update.
        currentState?.FixedTick();
    }

    private void UpdateFacing()
    {
        if (IsDead || !HasTarget || enemySprite == null)
        {
            return;
        }

        float differenceX = target.position.x - transform.position.x;

        if (Mathf.Abs(differenceX) <= FacingDeadZone)
        {
            return;
        }

        // The original sprite faces left. Flip it when the player is to the right.
        enemySprite.flipX = differenceX > 0f;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ChangeState(EnemyState nextState)
    {
        if (nextState == null || nextState == currentState)
        {
            return;
        }

        // A dead enemy cannot return to chasing or attacking.
        if (IsDead && !(nextState is EnemyDeadState))
        {
            return;
        }

        currentState?.Exit();

        currentState = nextState;
        currentState.Enter();
    }

    public bool IsTargetInAttackRange()
    {
        if (IsDead || !HasTarget)
        {
            return false;
        }

        Vector2 offset = (Vector2)target.position - rb.position;

        // Compare squared distances to avoid taking a square root.
        return offset.sqrMagnitude <= attackRange * attackRange;
    }

    public void MoveTowardsTarget()
    {
        if (IsDead || !HasTarget)
        {
            StopMoving();
            return;
        }

        Vector2 direction = (Vector2)target.position - rb.position;

        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    public void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public bool TryAttackTarget()
    {
        if (IsDead ||
            !isActiveAndEnabled ||
            !HasTarget ||
            !IsTargetInAttackRange())
        {
            return false;
        }

        if (Time.time < nextAttackTime)
        {
            return false;
        }

        nextAttackTime = Time.time + Mathf.Max(0.01f, attackCooldown);

        // Preview the attack without changing player health.
        Debug.Log(
            $"{name}: attacking {target.name}. " +
            $"Damage preview: {attackDamage}. " +
            "Player health is unchanged.",
            this
        );

        return true;
    }

    public void EnterDeadState()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        ChangeState(new EnemyDeadState(this));
    }

    private void OnDisable()
    {
        currentState?.Exit();
        currentState = null;

        StopMoving();
    }
}