using System;
using UnityEngine;

namespace TL6.PlayerCore
{
    [DisallowMultipleComponent]
    public sealed class PlayerHealthController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float invulnerabilitySeconds = 0.5f;
        private PlayerHealth health;

        public int CurrentHP => health.CurrentHP;
        public int MaxHP => health.MaxHP;
        public bool IsDead => health.IsDead;
        public bool IsInvulnerable => health.IsInvulnerable;
        public event Action PlayerDied;

        private void Awake()
        {
            health = new PlayerHealth(PlayerStats.Shared, invulnerabilitySeconds);
            health.PlayerDied += ReportDeath;
        }

        private void Update()
        {
            // Scaled time keeps the protection timer paused with the game.
            health.Tick(Time.deltaTime);
        }

        public void Initialise()
        {
            health.Initialise();
        }

        public int ApplyDamage(int amount)
        {
            return health.ApplyDamage(amount);
        }

        public int Heal(int amount)
        {
            return health.Heal(amount);
        }

        private void ReportDeath()
        {
            PlayerDied?.Invoke();
        }
    }
}
