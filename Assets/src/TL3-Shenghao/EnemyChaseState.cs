public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController enemy)
        : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.StopMoving();
    }

    public override void FixedTick()
    {
        if (!enemy.HasTarget)
        {
            enemy.StopMoving();
            return;
        }

        if (enemy.IsTargetInAttackRange())
        {
            enemy.ChangeState(new EnemyAttackState(enemy));
            return;
        }

        enemy.MoveTowardsTarget();
    }

    public override void Exit()
    {
        enemy.StopMoving();
    }
}