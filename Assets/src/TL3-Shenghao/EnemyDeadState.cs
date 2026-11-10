using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyController enemy)
        : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.StopMoving();

        Debug.Log(
            $"{enemy.name}: entered dead state.",
            enemy
        );
    }

    public override void FixedTick()
    {
        enemy.StopMoving();
    }
}