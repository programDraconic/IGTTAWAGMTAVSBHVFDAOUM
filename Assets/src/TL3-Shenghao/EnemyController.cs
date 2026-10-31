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

    private Rigidbody2D rb;
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

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        IsDead = false;
        nextAttackTime = float.NegativeInfinity;

        ChangeState(new EnemyChaseState(this));
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.FixedTick();
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

        Vector2 offset =
            (Vector2)target.position - rb.position;

        return offset.sqrMagnitude <= attackRange * attackRange;
    }

    public void MoveTowardsTarget()
    {
        if (IsDead || !HasTarget)
        {
            StopMoving();
            return;
        }

        Vector2 direction =
            (Vector2)target.position - rb.position;

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

        nextAttackTime = Time.time +
            Mathf.Max(0.01f, attackCooldown);

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