// Base class: calls made through Pickup are dispatched to the actual subclass.
public class Pickup
{
    public virtual string Collect(PlayerStats player)
    {
        return "Generic pickup collected";
    }
}
