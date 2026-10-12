public class XPPickup : Pickup
{
    public override string Collect(PlayerStats player)
    {
        player.XP += 25;
        return "Collected XP: +25";
    }
}
