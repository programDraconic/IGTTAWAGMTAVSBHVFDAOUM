using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnPadding = 2f;
    [SerializeField] private int maxEnemies = 20;

    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
    private float timer;

    private void Awake()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;
    }

    private void Update()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (player == null || enemyPrefab == null || gameCamera == null)
            return;

        if (spawnedEnemies.Count >= maxEnemies)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        float halfHeight = gameCamera.orthographicSize;
        float halfWidth = halfHeight * gameCamera.aspect;

        Vector2 offset;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                offset = new Vector2(-halfWidth - spawnPadding,
                    Random.Range(-halfHeight, halfHeight));
                break;
            case 1:
                offset = new Vector2(halfWidth + spawnPadding,
                    Random.Range(-halfHeight, halfHeight));
                break;
            case 2:
                offset = new Vector2(Random.Range(-halfWidth, halfWidth),
                    halfHeight + spawnPadding);
                break;
            default:
                offset = new Vector2(Random.Range(-halfWidth, halfWidth),
                    -halfHeight - spawnPadding);
                break;
        }

        Vector3 spawnPosition = gameCamera.transform.position
            + new Vector3(offset.x, offset.y, 0f);
        spawnPosition.z = player.position.z;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyChase chase = enemy.GetComponent<EnemyChase>();
        if (chase != null)
            chase.SetTarget(player);
        else
            Debug.LogError("The enemy prefab needs an EnemyChase component.");

        spawnedEnemies.Add(enemy);
    }
}