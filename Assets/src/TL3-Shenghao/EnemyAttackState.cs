public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyController enemy)
        : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.StopMoving();
    }

    public override void Tick()
    {
        if (!enemy.HasTarget ||
            !enemy.IsTargetInAttackRange())
        {
            enemy.ChangeState(new EnemyChaseState(enemy));
            return;
        }

        enemy.TryAttackTarget();
    }

    public override void FixedTick()
    {
        enemy.StopMoving();
    }

    public override void Exit()
    {
        enemy.StopMoving();
    }
}