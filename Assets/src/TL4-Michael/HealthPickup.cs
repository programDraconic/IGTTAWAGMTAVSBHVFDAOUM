public class HealthPickup : Pickup
{
    public override string Collect(PlayerStats player)
    {
        int before = player.Health;
        player.Health = System.Math.Min(player.MaxHealth, player.Health + 20);
        return "Collected health: +" + (player.Health - before);
    }
}
