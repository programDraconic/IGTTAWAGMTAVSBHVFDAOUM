public abstract class EnemyState
{
    protected readonly EnemyController enemy;

    protected EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
    }

    public virtual void Tick()
    {
    }

    public virtual void FixedTick()
    {
    }

    public virtual void Exit()
    {
    }
}