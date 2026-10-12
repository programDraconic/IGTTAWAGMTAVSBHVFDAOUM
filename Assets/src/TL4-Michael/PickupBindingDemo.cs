using UnityEngine;

// Attach this script to an empty GameObject in a Unity scene.
public class PickupBindingDemo : MonoBehaviour
{
    // Declared type: Pickup. Actual runtime type: XPPickup (initially).
    private Pickup current = new XPPickup();
    private readonly PlayerStats player = new PlayerStats();
    private int pickupIndex = 0;
    private string lastResult = "Click Collect to use the selected pickup.";

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 480, 30), "Declared type: Pickup | Actual type: " + current.GetType().Name);
        GUI.Label(new Rect(20, 50, 600, 30), lastResult);
        GUI.Label(new Rect(20, 80, 650, 30),
            "XP: " + player.XP + " | Shards: " + player.Shards +
            " | Health: " + player.Health + "/" + player.MaxHealth +
            " | Other: " + player.OtherRewards);

        if (GUI.Button(new Rect(20, 120, 160, 35), "Collect"))
        {
            // One call; C# selects the correct override at runtime.
            lastResult = current.Collect(player);
        }

        if (GUI.Button(new Rect(190, 120, 160, 35), "Swap Pickup"))
        {
            pickupIndex = (pickupIndex + 1) % 4;
            switch (pickupIndex)
            {
                case 0: current = new XPPickup(); break;
                case 1: current = new ShardPickup(); break;
                case 2: current = new HealthPickup(); break;
                default: current = new OtherPickup(); break;
            }
            lastResult = "Switched to " + current.GetType().Name;
        }
    }
}
