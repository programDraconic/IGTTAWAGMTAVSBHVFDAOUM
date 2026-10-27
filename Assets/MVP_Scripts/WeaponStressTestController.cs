using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStressTestController : MonoBehaviour
{
    [SerializeField] private PlayerWeaponController weaponController;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private TMP_Text stressPointText;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text breakPointText;

    [Header("Test settings")]
    [SerializeField] private int startingEnemyCount = 16;
    [SerializeField] private int maximumEnemyCount = 8192;
    [SerializeField] private float spawnRadius = 1.4f;
    [SerializeField] private float secondsPerStage = 2f;
    [SerializeField] private float secondsBetweenAttacks = 0.25f;

    private int spawnedCount;
    private bool previousLoggingState;

    private void Start()
    {
        if (weaponController == null || enemyPrefab == null ||
            stressPointText == null || fpsText == null || breakPointText == null)
        {
            Debug.LogError("Assign the weapon, enemy prefab, and all three Text fields.");
            enabled = false;
            return;
        }

        StartCoroutine(RunStressTest());
    }

    private IEnumerator RunStressTest()
    {
        // Disable per-shot Debug.Log calls so console output doesn't skew FPS.
        previousLoggingState = Debug.unityLogger.logEnabled;
        Debug.unityLogger.logEnabled = false;

        breakPointText.text = "Break Point: measuring baseline";
        stressPointText.text = "Stress Point: starting";
        fpsText.text = "FPS: measuring";

        float elapsed = 0f;
        int frames = 0;

        while (elapsed < 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            frames++;
            yield return null;
        }

        float startingFps = frames / elapsed;
        fpsText.text = $"FPS: {startingFps:F0} (baseline)";

        for (int targetCount = startingEnemyCount;
             targetCount <= maximumEnemyCount;
             targetCount *= 2)
        {
            SpawnUntil(targetCount);
            stressPointText.text = $"Stress Point: {spawnedCount} enemies";

            elapsed = 0f;
            frames = 0;
            float attackTimer = 0f;

            while (elapsed < secondsPerStage)
            {
                float delta = Time.unscaledDeltaTime;
                elapsed += delta;
                frames++;
                attackTimer += delta;

                if (attackTimer >= secondsBetweenAttacks)
                {
                    weaponController.StressTestFire();
                    attackTimer = 0f;
                }

                fpsText.text = $"FPS: {(1f / delta):F0}";
                yield return null;
            }

            float averageFps = frames / elapsed;
            fpsText.text = $"FPS: {averageFps:F0} (stage average)";

            if (averageFps <= startingFps * 0.5f)
            {
                breakPointText.text =
                    $"Break Point: {spawnedCount} enemies at {averageFps:F0} FPS";
                Debug.unityLogger.logEnabled = previousLoggingState;
                yield break;
            }
        }

        breakPointText.text =
            $"Break Point: not reached by {maximumEnemyCount} enemies";
        Debug.unityLogger.logEnabled = previousLoggingState;
    }

    private void SpawnUntil(int targetCount)
    {
        while (spawnedCount < targetCount)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 position = weaponController.transform.position
                               + new Vector3(offset.x, offset.y, 0f);

            Instantiate(enemyPrefab, position, Quaternion.identity, transform);
            spawnedCount++;
        }
    }

    private void OnDestroy()
    {
        Debug.unityLogger.logEnabled = previousLoggingState;
    }
}
