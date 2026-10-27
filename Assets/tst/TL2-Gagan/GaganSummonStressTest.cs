using System.Collections.Generic;
using UnityEngine;

public class GaganSummonStressTest : MonoBehaviour
{
    [Header("Gagan Summon Prefab")]
    [SerializeField] private GameObject summonPrefab;

    [Header("Stress Test Settings")]
    [SerializeField] private int startingSummons = 1;
    [SerializeField] private int maximumSummons = 4096;
    [SerializeField] private float baselineSeconds = 3f;
    [SerializeField] private float escalationSeconds = 1.5f;
    [SerializeField] private Vector2 testArea = new Vector2(16f, 8f);
    [SerializeField] private float summonSpeed = 2.5f;

    private readonly List<GameObject> spawnedSummons =
        new List<GameObject>();

    private readonly List<Summon> summonModels =
        new List<Summon>();

    private SummonContext stressContext;
    private PhysicsMaterial2D bounceMaterial;

    private float currentFPS;
    private float startingFPS;
    private float displaySampleTime;
    private int displaySampleFrames;

    private float baselineTime;
    private int baselineFrames;
    private float escalationTimer;
    private float physicsCheckTimer;

    private int lowFPSSamples;
    private bool baselineReady;
    private bool testFinished;

    private string breakPoint = "Testing has not reached a break.";
    private string hardwareInformation;

    private int previousVSync;
    private int previousTargetFrameRate;

    private GUIStyle labelStyle;
    private GUIStyle headingStyle;

    private void Start()
    {
        previousVSync = QualitySettings.vSyncCount;
        previousTargetFrameRate = Application.targetFrameRate;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        hardwareInformation =
            SystemInfo.processorType + " | " +
            SystemInfo.graphicsDeviceName + " | RAM: " +
            SystemInfo.systemMemorySize + " MB";

        stressContext = new SummonContext(
            Vector2.zero,
            "StressTarget",
            true);

        bounceMaterial =
            new PhysicsMaterial2D("Gagan Stress Bounce");

        bounceMaterial.friction = 0f;
        bounceMaterial.bounciness = 1f;

        CreatePhysicsBoundaries();

        if (summonPrefab == null)
        {
            FinishTest(
                "SETUP ERROR: Gagan summon prefab is not assigned.");
            return;
        }

        SpawnUntil(startingSummons);

        Debug.Log(
            "Gagan Summon Stress Test started.\nHardware: " +
            hardwareInformation);
    }

    private void Update()
    {
        UpdateFPSMeasurement();
        RunSummonBehavior();

        if (testFinished)
        {
            return;
        }

        if (!baselineReady)
        {
            baselineTime += Time.unscaledDeltaTime;
            baselineFrames++;

            if (baselineTime >= baselineSeconds)
            {
                startingFPS = baselineFrames / baselineTime;
                baselineReady = true;

                Debug.Log(
                    "Starting FPS recorded: " +
                    startingFPS.ToString("F1"));
            }

            return;
        }

        escalationTimer += Time.unscaledDeltaTime;
        physicsCheckTimer += Time.unscaledDeltaTime;

        if (physicsCheckTimer >= 0.25f)
        {
            physicsCheckTimer = 0f;
            CheckForPhysicsFailure();
        }

        if (escalationTimer >= escalationSeconds)
        {
            escalationTimer = 0f;

            int nextStressPoint = Mathf.Min(
                spawnedSummons.Count * 2,
                maximumSummons);

            SpawnUntil(nextStressPoint);

            if (spawnedSummons.Count >= maximumSummons)
            {
                FinishTest(
                    "Bounded limit reached at " +
                    spawnedSummons.Count +
                    " summons. FPS: " +
                    currentFPS.ToString("F1"));
            }
        }
    }

    private void UpdateFPSMeasurement()
    {
        displaySampleTime += Time.unscaledDeltaTime;
        displaySampleFrames++;

        if (displaySampleTime < 0.5f)
        {
            return;
        }

        currentFPS = displaySampleFrames / displaySampleTime;
        displaySampleTime = 0f;
        displaySampleFrames = 0;

        if (!baselineReady || testFinished)
        {
            return;
        }

        float fpsBreakLevel = startingFPS * 0.5f;

        if (currentFPS <= fpsBreakLevel)
        {
            lowFPSSamples++;
        }
        else
        {
            lowFPSSamples = 0;
        }

        // Three low samples prevent a single slow frame
        // from incorrectly ending the test.
        if (lowFPSSamples >= 3)
        {
            FinishTest(
                "FPS break at " +
                spawnedSummons.Count +
                " summons. Starting FPS: " +
                startingFPS.ToString("F1") +
                ", current FPS: " +
                currentFPS.ToString("F1"));
        }
    }

    private void RunSummonBehavior()
    {
        // This exercises the real Summon hierarchy and
        // its dynamically bound UpdateBehavior method.
        for (int i = 0; i < summonModels.Count; i++)
        {
            summonModels[i].UpdateBehavior(stressContext);
        }
    }

    private void SpawnUntil(int targetCount)
    {
        while (spawnedSummons.Count < targetCount)
        {
            SpawnOneSummon();
        }
    }

    private void SpawnOneSummon()
    {
        float halfWidth = testArea.x * 0.5f - 1f;
        float halfHeight = testArea.y * 0.5f - 1f;

        Vector3 position = new Vector3(
            Random.Range(-halfWidth, halfWidth),
            Random.Range(-halfHeight, halfHeight),
            0f);

        GameObject clone = Instantiate(
            summonPrefab,
            position,
            Quaternion.identity);

        clone.name =
            "GaganStressSummon_" + spawnedSummons.Count;

        Rigidbody2D body = clone.GetComponent<Rigidbody2D>();

        if (body == null)
        {
            body = clone.AddComponent<Rigidbody2D>();
        }

        Collider2D summonCollider =
            clone.GetComponent<Collider2D>();

        if (summonCollider == null)
        {
            summonCollider =
                clone.AddComponent<CircleCollider2D>();
        }

        summonCollider.sharedMaterial = bounceMaterial;

        body.gravityScale = 0f;
        body.linearDamping = 0f;
        body.angularDamping = 0f;
        body.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        Vector2 direction = Random.insideUnitCircle;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        body.linearVelocity =
            direction.normalized * summonSpeed;

        spawnedSummons.Add(clone);

        Summon model;

        switch (summonModels.Count % 4)
        {
            case 0:
                model = new OrbitingSummon();
                break;

            case 1:
                model = new HuntingSummon();
                break;

            case 2:
                model = new TotemSummon();
                break;

            default:
                model = new RevengeSummon();
                break;
        }

        model.Activate();
        summonModels.Add(model);
    }

    private void CheckForPhysicsFailure()
    {
        float allowedX = testArea.x * 0.5f + 1.5f;
        float allowedY = testArea.y * 0.5f + 1.5f;

        for (int i = 0; i < spawnedSummons.Count; i++)
        {
            GameObject summon = spawnedSummons[i];

            if (summon == null)
            {
                FinishTest(
                    "Physics break: a summon was unexpectedly destroyed.");
                return;
            }

            Vector3 position = summon.transform.position;

            if (Mathf.Abs(position.x) > allowedX ||
                Mathf.Abs(position.y) > allowedY)
            {
                FinishTest(
                    "Physics break at " +
                    spawnedSummons.Count +
                    " summons: a summon escaped the boundaries.");
                return;
            }
        }
    }

    private void CreatePhysicsBoundaries()
    {
        float halfWidth = testArea.x * 0.5f;
        float halfHeight = testArea.y * 0.5f;

        CreateWall(
            "Left Physics Wall",
            new Vector2(-halfWidth - 0.25f, 0f),
            new Vector2(0.5f, testArea.y + 1f));

        CreateWall(
            "Right Physics Wall",
            new Vector2(halfWidth + 0.25f, 0f),
            new Vector2(0.5f, testArea.y + 1f));

        CreateWall(
            "Top Physics Wall",
            new Vector2(0f, halfHeight + 0.25f),
            new Vector2(testArea.x + 1f, 0.5f));

        CreateWall(
            "Bottom Physics Wall",
            new Vector2(0f, -halfHeight - 0.25f),
            new Vector2(testArea.x + 1f, 0.5f));
    }

    private void CreateWall(
        string wallName,
        Vector2 position,
        Vector2 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.SetParent(transform);
        wall.transform.position = position;

        BoxCollider2D wallCollider =
            wall.AddComponent<BoxCollider2D>();

        wallCollider.size = size;
        wallCollider.sharedMaterial = bounceMaterial;
    }

    private void FinishTest(string reason)
    {
        if (testFinished)
        {
            return;
        }

        testFinished = true;
        breakPoint = reason;

        Debug.Log(
            "GAGAN STRESS TEST FINISHED\n" +
            reason + "\nHardware: " +
            hardwareInformation);
    }

    private void OnGUI()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 18;
            labelStyle.wordWrap = true;
            labelStyle.normal.textColor = Color.white;

            headingStyle = new GUIStyle(labelStyle);
            headingStyle.fontSize = 22;
            headingStyle.fontStyle = FontStyle.Bold;
        }

        GUILayout.BeginArea(
            new Rect(20f, 20f, 650f, 250f),
            GUI.skin.box);

        GUILayout.Label(
            "GAGAN SUMMON STRESS TEST",
            headingStyle);

        GUILayout.Label(
            "Current Stress Point: " +
            spawnedSummons.Count +
            " active summons",
            labelStyle);

        GUILayout.Label(
            "Current FPS: " +
            currentFPS.ToString("F1"),
            labelStyle);

        if (baselineReady)
        {
            GUILayout.Label(
                "Starting FPS: " +
                startingFPS.ToString("F1") +
                " | FPS Break Level: " +
                (startingFPS * 0.5f).ToString("F1"),
                labelStyle);
        }
        else
        {
            GUILayout.Label(
                "Starting FPS: measuring...",
                labelStyle);
        }

        GUILayout.Label(
            "Break Point: " + breakPoint,
            labelStyle);

        GUILayout.Label(
            "Hardware: " + hardwareInformation,
            labelStyle);

        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        QualitySettings.vSyncCount = previousVSync;
        Application.targetFrameRate =
            previousTargetFrameRate;

        if (bounceMaterial != null)
        {
            Destroy(bounceMaterial);
        }
    }
}
