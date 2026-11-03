using UnityEngine;

namespace TL6.PlayerCore
{
    [RequireComponent(typeof(PlayerHealthController))]
    public sealed class PlayerHealthDemo : MonoBehaviour
    {
        private PlayerHealthController health;
        private string lastAction = "Ready. Take damage to try the health system.";

        private void Start()
        {
            health = GetComponent<PlayerHealthController>();
            health.PlayerDied += ShowDeath;
            health.Initialise();
        }

        private void OnGUI()
        {
            if (health == null) return;

            GUILayout.BeginArea(new Rect(20, 20, 370, 350), GUI.skin.box);
            GUILayout.Label("Player Health");
            GUILayout.Label("HP: " + health.CurrentHP + " / " + health.MaxHP);
            GUILayout.Label("Armour: " + PlayerStats.Shared.Armor);
            GUILayout.Label(health.IsDead ? "Player died" :
                health.IsInvulnerable ? "Protected after a hit" : "Ready for damage");
            GUILayout.Space(10);

            GUI.enabled = !health.IsDead;
            if (GUILayout.Button("Take 10 damage"))
            {
                int lost = health.ApplyDamage(10);
                if (!health.IsDead)
                    lastAction = lost == 0 ? "Hit blocked by armour or protection." :
                        "Lost " + lost + " HP.";
            }

            if (GUILayout.Button("Heal 20"))
                lastAction = "Recovered " + health.Heal(20) + " HP.";

            GUI.enabled = true;
            if (GUILayout.Button("Start new run"))
            {
                health.Initialise();
                lastAction = "New run: health restored.";
            }

            GUILayout.Space(10);
            GUILayout.Label(lastAction);
            GUILayout.Label("This demo uses PlayerStats.Shared.");
            GUILayout.EndArea();
        }

        private void ShowDeath()
        {
            lastAction = "PlayerDied event raised. Start a new run to try again.";
            Debug.Log("Player health: PlayerDied event raised.", this);
        }

        private void OnDestroy()
        {
            if (health != null) health.PlayerDied -= ShowDeath;
        }
    }
}
