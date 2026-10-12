public class ShardPickup : Pickup
{
    public override string Collect(PlayerStats player)
    {
        player.Shards += 3;
        return "Collected shards: +3";
    }
}
