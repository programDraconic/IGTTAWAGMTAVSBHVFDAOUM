using System;

namespace TL6.PlayerCore
{
    // Health rules operate on the same stats used by pickups and upgrades.
    public sealed class PlayerHealth
    {
        private readonly PlayerStats stats;
        private readonly float invulnerabilitySeconds;
        private float protectionRemaining;
        private bool deathReported;

        public int CurrentHP => Math.Max(0, stats.Health);
        public int MaxHP => Math.Max(1, stats.MaxHealth);
        public bool IsDead => deathReported || CurrentHP == 0;
        public bool IsInvulnerable => protectionRemaining > 0f;
        public event Action PlayerDied;

        public PlayerHealth(PlayerStats stats, float invulnerabilitySeconds = 0.5f)
        {
            if (stats == null) throw new ArgumentNullException(nameof(stats));
            if (invulnerabilitySeconds < 0f || float.IsNaN(invulnerabilitySeconds) ||
                float.IsInfinity(invulnerabilitySeconds))
                throw new ArgumentOutOfRangeException(nameof(invulnerabilitySeconds));

            this.stats = stats;
            this.invulnerabilitySeconds = invulnerabilitySeconds;
        }

        // Call for a new run, not for every room or scene transition.
        public void Initialise()
        {
            stats.MaxHealth = MaxHP;
            stats.Health = MaxHP;
            protectionRemaining = 0f;
            deathReported = false;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
                return;

            protectionRemaining = Math.Max(0f, protectionRemaining - deltaTime);
        }

        // Returns HP actually lost, after armour and the protection window.
        public int ApplyDamage(int amount)
        {
            if (amount <= 0 || IsDead || IsInvulnerable) return 0;

            int damage = Math.Max(0, amount - Math.Max(0, stats.Armor));
            int lostHP = Math.Min(CurrentHP, damage);
            if (lostHP == 0) return 0;

            stats.Health = CurrentHP - lostHP;
            protectionRemaining = invulnerabilitySeconds;

            if (stats.Health == 0)
            {
                deathReported = true;
                PlayerDied?.Invoke();
            }

            return lostHP;
        }

        // Healing cannot exceed maximum HP or revive a completed run.
        public int Heal(int amount)
        {
            if (amount <= 0 || IsDead) return 0;

            int gainedHP = Math.Min(amount, Math.Max(0, MaxHP - CurrentHP));
            stats.Health = CurrentHP + gainedHP;
            return gainedHP;
        }
    }
}
