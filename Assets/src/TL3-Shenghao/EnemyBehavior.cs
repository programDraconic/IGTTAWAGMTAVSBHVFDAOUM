using System;
using UnityEngine;

namespace ShrimpGame
{
    public enum EnemyChaseRule
    {
        Never,
        WithinRange,
        Always
    }

    [DisallowMultipleComponent]
    public sealed class EnemyBehavior : MonoBehaviour
    {
        [Serializable]
        private struct WeaponModifier
        {
            public string weaponId;
            [Min(0f)] public float multiplier;
        }

        [SerializeField, Min(0f)] private float moveSpeed = 2f;
        [SerializeField] private EnemyChaseRule chaseRule = EnemyChaseRule.WithinRange;
        [SerializeField, Min(0f)] private float detectionRadius = 5f;
        [SerializeField, Min(0f)] private float stoppingDistance = 0.25f;
        [SerializeField] private WeaponModifier[] weaponModifiers = Array.Empty<WeaponModifier>();

        public float MoveSpeed => Mathf.Max(0f, moveSpeed);

        public float GetDamageFromWeapon(string weaponId, float baseDamage)
        {
            float damage = Mathf.Max(0f, baseDamage);
            if (string.IsNullOrWhiteSpace(weaponId))
                return damage;

            foreach (WeaponModifier modifier in weaponModifiers)
            {
                if (string.Equals(modifier.weaponId, weaponId, StringComparison.Ordinal))
                    return damage * Mathf.Max(0f, modifier.multiplier);
            }

            return damage;
        }

        // Returns a velocity in world units per second on the XY plane.
        // False lets the caller fall back to its existing roaming behavior.
        public bool TryGetChaseVelocity(Transform target, out Vector2 velocity)
        {
            velocity = Vector2.zero;
            if (target == null || chaseRule == EnemyChaseRule.Never)
                return false;

            Vector2 offset = (Vector2)(target.position - transform.position);
            float distance = offset.magnitude;
            if (chaseRule == EnemyChaseRule.WithinRange && distance > detectionRadius)
                return false;

            if (distance > stoppingDistance)
                velocity = offset.normalized * MoveSpeed;

            return true;
        }
    }
}
