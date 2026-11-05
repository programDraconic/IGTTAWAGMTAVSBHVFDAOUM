// Created by ChatGPT for the Shrimp Game project.
// Activates hand-placed enemies once a position or time condition is met.

using UnityEngine;

public class PlacedEnemyActivator : MonoBehaviour
{
    private enum PositionRule
    {
        None,
        XAtLeast,
        XAtMost,
        YAtLeast,
        YAtMost,
        InsideXYArea
    }

    [Header("Scene References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyController[] placedEnemies;

    [Header("Position Trigger")]
    [SerializeField] private PositionRule positionRule = PositionRule.None;
    [SerializeField] private float positionThreshold;
    [SerializeField] private Vector2 areaMin;
    [SerializeField] private Vector2 areaMax;

    [Header("Time Trigger")]
    [SerializeField] private bool useTimeTrigger;
    [SerializeField, Min(0f)] private float delaySeconds = 10f;

    private bool activated;
    private float elapsed;

    private void Update()
    {
        if (activated || player == null ||
            !player.gameObject.activeInHierarchy)
        {
            return;
        }

        elapsed += Time.deltaTime;

        bool positionReached = PositionMatches(player.position);
        bool timeReached = useTimeTrigger && elapsed >= delaySeconds;

        // If both triggers are enabled, whichever happens first wins.
        if (!positionReached && !timeReached)
        {
            return;
        }

        activated = true;

        if (placedEnemies == null)
        {
            return;
        }

        foreach (EnemyController enemy in placedEnemies)
        {
            if (enemy == null) continue;

            // Assign the target before the enemy's OnEnable runs.
            enemy.SetTarget(player);
            enemy.gameObject.SetActive(true);
        }
    }

    private bool PositionMatches(Vector2 position)
    {
        switch (positionRule)
        {
            case PositionRule.XAtLeast:
                return position.x >= positionThreshold;

            case PositionRule.XAtMost:
                return position.x <= positionThreshold;

            case PositionRule.YAtLeast:
                return position.y >= positionThreshold;

            case PositionRule.YAtMost:
                return position.y <= positionThreshold;

            case PositionRule.InsideXYArea:
                Vector2 min = Vector2.Min(areaMin, areaMax);
                Vector2 max = Vector2.Max(areaMin, areaMax);

                return position.x >= min.x && position.x <= max.x &&
                       position.y >= min.y && position.y <= max.y;

            default:
                return false;
        }
    }
}