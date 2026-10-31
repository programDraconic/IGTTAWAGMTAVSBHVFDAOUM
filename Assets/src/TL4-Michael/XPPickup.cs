public class XPPickup : Pickup
{
    public override string Collect(PlayerStats player)
    {
        player.GainXP(25);
        return "Collected XP: +25";
    }
}
