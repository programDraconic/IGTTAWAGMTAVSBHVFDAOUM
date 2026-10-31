// Crystal/shard pickups also award experience through the shared leveling system.
public class ShardPickup : Pickup
{
    public const int ShardsPerPickup = 3;
    public const int XPPerPickup = 25;

    public override string Collect(PlayerStats player)
    {
        // Use the passed-in stats so this works in tests and with PlayerStats.Shared.
        player.Shards += ShardsPerPickup;
        player.GainXP(XPPerPickup); // Triggers level-ups, pending cards, and XPChanged.
        return "Collected crystal: +" + ShardsPerPickup +
               " shards, +" + XPPerPickup + " XP";
    }
}
