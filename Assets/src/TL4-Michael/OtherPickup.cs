public class OtherPickup : Pickup
{
    public override string Collect(PlayerStats player)
    {
        player.OtherRewards++;
        return "Collected a special reward";
    }
}
