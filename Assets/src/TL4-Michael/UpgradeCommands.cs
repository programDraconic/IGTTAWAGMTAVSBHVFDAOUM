// COMMAND PATTERN: every level-up action implements this interface.
// The UI invokes Execute() without knowing which stat an upgrade modifies.
public interface IUpgradeCommand
{
    void Execute(PlayerStats player);
}

public sealed class PowerStrikeCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.Damage += 5; }
}

public sealed class IronSkinCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.Armor += 3; }
}

public sealed class FleetFootedCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.MoveSpeed += 0.5f; }
}

public sealed class RapidAttackCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.AttackSpeed += 0.2f; }
}

public sealed class VitalityCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player)
    {
        player.MaxHealth += 20;
        player.Heal(20);
    }
}

public sealed class SecondWindCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.Heal(40); }
}

public sealed class TreasureHunterCommand : IUpgradeCommand
{
    public void Execute(PlayerStats player) { player.Shards += 10; }
}
