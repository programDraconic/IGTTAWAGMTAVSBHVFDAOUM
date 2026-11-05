// Created by ChatGPT for the Shrimp Game project.
// Spawns an initial group, then continues spawning on a timer.

using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private enum ConditionType
    {
        None,
        ElapsedSeconds,
        PlayerXAtLeast,
        PlayerXAtMost,
        PlayerYAtLeast,
        PlayerYAtMost,
        PlayerInXYArea,
        PlayerOutsideXYArea,
        AliveAtLeast,
        AliveAtMost
    }

    [Serializable]
    private class Condition
    {
        public ConditionType type;
        public float value;
        public Vector2 areaMin;
        public Vector2 areaMax;

        public bool Matches(Vector2 position, float elapsed, int alive)
        {
            switch (type)
            {
                case ConditionType.ElapsedSeconds:
                    return elapsed >= value;

                case ConditionType.PlayerXAtLeast:
                    return position.x >= value;

                case ConditionType.PlayerXAtMost:
                    return position.x <= value;

                case ConditionType.PlayerYAtLeast:
                    return position.y >= value;

                case ConditionType.PlayerYAtMost:
                    return position.y <= value;

                case ConditionType.PlayerInXYArea:
                    return IsInsideArea(position);

                case ConditionType.PlayerOutsideXYArea:
                    return !IsInsideArea(position);

                case ConditionType.AliveAtLeast:
                    return alive >= value;

                case ConditionType.AliveAtMost:
                    return alive <= value;

                default:
                    return false;
            }
        }

        private bool IsInsideArea(Vector2 position)
        {
            Vector2 min = Vector2.Min(areaMin, areaMax);
            Vector2 max = Vector2.Max(areaMin, areaMax);

            return position.x >= min.x && position.x <= max.x &&
                   position.y >= min.y && position.y <= max.y;
        }
    }

    [Serializable]
    private class IntervalOverride
    {
        public Condition when = new Condition();
        [Min(0.1f)] public float secondsPerSpawn = 2f;
    }

    [Serializable]
    private class EnemySpawnSettings
    {
        public EnemyController enemyPrefab;
        public Transform[] spawnPoints;

        [Min(0.1f)] public float spawnInterval = 3f;
        [Min(0f)] public float minDistanceFromPlayer = 4f;
        [Min(1)] public int maxAlive = 5;

        [Tooltip("Enemies to spawn immediately when this entry starts.")]
        [Min(0)] public int initialSpawnCount = 0;

        [Tooltip("Includes the initial group. Zero means unlimited.")]
        [Min(0)] public int maxTotalSpawns = 0;

        public Condition startRule = new Condition();
        public Condition stopRule = new Condition();
        public Condition activeWhile = new Condition();
        public IntervalOverride[] intervalOverrides;

        [NonSerialized] public bool started;
        [NonSerialized] public bool stopped;
        [NonSerialized] public bool initialSpawnHandled;
        [NonSerialized] public float spawnProgress;
        [NonSerialized] public int totalSpawned;

        [NonSerialized]
        public List<EnemyController> spawnedEnemies =
            new List<EnemyController>();
    }

    [SerializeField] private Transform player;
    [SerializeField] private EnemySpawnSettings[] enemyTypes;

    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;

        if (player == null || !player.gameObject.activeInHierarchy ||
            enemyTypes == null)
        {
            return;
        }

        Vector2 playerPosition = player.position;

        foreach (EnemySpawnSettings settings in enemyTypes)
        {
            if (settings == null || settings.stopped ||
                settings.enemyPrefab == null || settings.spawnPoints == null)
            {
                continue;
            }

            settings.spawnedEnemies ??= new List<EnemyController>();
            settings.spawnedEnemies.RemoveAll(
                enemy => enemy == null || enemy.IsDead
            );

            int alive = settings.spawnedEnemies.Count;

            if (!settings.started)
            {
                if (HasRule(settings.startRule) &&
                    !settings.startRule.Matches(playerPosition, elapsed, alive))
                {
                    continue;
                }

                settings.started = true;
            }

            // A stop rule takes priority over any spawn due this frame.
            if (HasRule(settings.stopRule) &&
                settings.stopRule.Matches(playerPosition, elapsed, alive))
            {
                settings.stopped = true;
                continue;
            }

            if (TotalLimitReached(settings))
            {
                settings.stopped = true;
                continue;
            }

            // Pause while the active condition is false or the enemy cap is full.
            if ((HasRule(settings.activeWhile) &&
                 !settings.activeWhile.Matches(playerPosition, elapsed, alive)) ||
                alive >= settings.maxAlive)
            {
                continue;
            }

            if (!settings.initialSpawnHandled)
            {
                if (settings.initialSpawnCount <= 0)
                {
                    settings.initialSpawnHandled = true;
                }
                else
                {
                    // Wait for an eligible point if all points are too close.
                    if (!TrySpawnEnemy(settings, playerPosition))
                    {
                        continue;
                    }

                    // Once the group starts, spawn as many as the limits allow.
                    settings.initialSpawnHandled = true;

                    for (int i = 1; i < settings.initialSpawnCount; i++)
                    {
                        if (!TrySpawnEnemy(settings, playerPosition))
                        {
                            break;
                        }
                    }

                    // Begin the normal countdown on the next frame.
                    continue;
                }
            }

            float interval = GetInterval(settings, playerPosition, alive);
            settings.spawnProgress += Time.deltaTime / interval;

            if (settings.spawnProgress >= 1f)
            {
                settings.spawnProgress = 0f;
                TrySpawnEnemy(settings, playerPosition);
            }
        }
    }

    private bool TrySpawnEnemy(
        EnemySpawnSettings settings, Vector2 playerPosition)
    {
        if (settings.spawnedEnemies.Count >= settings.maxAlive ||
            TotalLimitReached(settings))
        {
            return false;
        }

        Transform point = ChooseSpawnPoint(settings, playerPosition);
        if (point == null)
        {
            return false;
        }

        EnemyController enemy = Instantiate(
            settings.enemyPrefab, point.position, point.rotation
        );

        enemy.SetTarget(player);
        settings.spawnedEnemies.Add(enemy);
        settings.totalSpawned++;

        return true;
    }

    private static bool TotalLimitReached(EnemySpawnSettings settings)
    {
        return settings.maxTotalSpawns > 0 &&
               settings.totalSpawned >= settings.maxTotalSpawns;
    }

    private static bool HasRule(Condition rule)
    {
        return rule != null && rule.type != ConditionType.None;
    }

    private float GetInterval(
        EnemySpawnSettings settings, Vector2 playerPosition, int alive)
    {
        if (settings.intervalOverrides != null)
        {
            // The first matching override wins.
            foreach (IntervalOverride rule in settings.intervalOverrides)
            {
                if (rule != null && HasRule(rule.when) &&
                    rule.when.Matches(playerPosition, elapsed, alive))
                {
                    return Mathf.Max(0.1f, rule.secondsPerSpawn);
                }
            }
        }

        return Mathf.Max(0.1f, settings.spawnInterval);
    }

    private static Transform ChooseSpawnPoint(
        EnemySpawnSettings settings, Vector2 playerPosition)
    {
        Transform chosenPoint = null;
        int eligibleCount = 0;
        float minDistance = Mathf.Max(0f, settings.minDistanceFromPlayer);
        float minDistanceSquared = minDistance * minDistance;

        foreach (Transform point in settings.spawnPoints)
        {
            if (point == null) continue;

            Vector2 offset = (Vector2)point.position - playerPosition;
            if (offset.sqrMagnitude < minDistanceSquared) continue;

            // Give every eligible point an equal chance of being selected.
            eligibleCount++;

            if (UnityEngine.Random.Range(0, eligibleCount) == 0)
            {
                chosenPoint = point;
            }
        }

        return chosenPoint;
    }
}