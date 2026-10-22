using System.Collections.Generic;
using TMPro;
using UnityEngine;

// The only MonoBehaviour of the run-state system: bridges Unity's Update
// loop to the plain-C# session and freezes Unity while not Playing.
// Also hosts the stress-test spawner (debug tooling, safe to disable).
public class RunSessionRunner : MonoBehaviour
{
    public static RunSessionRunner Instance { get; private set; }

    [SerializeField] private float countdownSeconds = 3f;

    [Header("Stress test (optional)")]
    [SerializeField] private GameObject stressPrefab;        // drag the StressDummy PREFAB here
    [SerializeField] private TMP_Text numberOfObjectsViewer; // TMP text inside a Canvas
    [SerializeField] private TMP_Text fpsCounter;            // TMP text inside a Canvas
    [SerializeField] private bool autoStartStressTest = false;
    [SerializeField] private int targetFps = 60;             // stop once fps drops to this...
    [SerializeField] private int minObjects = 20;            // ...and at least this many exist
    [SerializeField] private int spawnPerFrame = 1;
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float fpsSampleSeconds = 0.25f; // averaging window, smooths spikes
    [SerializeField] private bool uncapFrameRate = true;     // turn off vsync/frame cap while testing

    [Header("Debug (optional)")]
    [SerializeField] private bool showDebugGui = true;

    public RunSession Session { get; private set; }

    // Stress-test state
    private readonly List<GameObject> spawned = new List<GameObject>();
    private bool stressRunning;
    private bool haveFps;
    private int currentFps;
    private float fpsTimer;
    private int fpsFrames;
    private int prevVSync;
    private int prevTargetFrameRate;

    void Awake()
    {
        Instance = this;
        Session = new RunSession();
        Session.PhaseChanged += OnPhaseChanged;
        RunHandle handle = Session.Initialize(new RunConfig { CountdownSeconds = countdownSeconds });
        Session.StartRun(handle.RunId);
    }

    void Start()
    {
        if (autoStartStressTest) StartStressTest();
        RefreshHud();
    }

    void Update()
    {
        // Unscaled so the Ready countdown still advances while timeScale is 0.
        Session.Tick(new FrameContext(Time.unscaledDeltaTime, Time.unscaledTime));

        SampleFps();
        UpdateStressTest();
    }

    // ---------------- Run state ----------------

    void OnPhaseChanged(RunState state)
    {
        Time.timeScale = state.RunsSimulation ? 1f : 0f;
        AudioListener.pause = !state.RunsSimulation;
        // Show/hide pause menu, level-up panel, HUD here too.
    }

    // Wire this to your pause input (Input System action, UI button, etc.).
    public void OnPausePressed()
    {
        if (Session.CurrentState is PausedState)
            Session.Resume();
        else
            Session.RequestPause(PauseReason.Player);
    }

    // ---------------- Stress test ----------------

    public void StartStressTest()
    {
        if (stressPrefab == null)
        {
            Debug.LogWarning("RunSessionRunner: assign the StressDummy prefab to Stress Prefab.");
            return;
        }
        if (stressRunning) return;

        if (uncapFrameRate)
        {
            prevVSync = QualitySettings.vSyncCount;
            prevTargetFrameRate = Application.targetFrameRate;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }
        stressRunning = true;
    }

    public void StopStressTest()
    {
        if (!stressRunning) return;
        stressRunning = false;

        if (uncapFrameRate)
        {
            QualitySettings.vSyncCount = prevVSync;
            Application.targetFrameRate = prevTargetFrameRate;
        }
    }

    public void ClearSpawned()
    {
        for (int i = 0; i < spawned.Count; i++)
            if (spawned[i] != null) Destroy(spawned[i]);
        spawned.Clear();
        RefreshHud();
    }

    void SampleFps()
    {
        fpsTimer += Time.unscaledDeltaTime;
        fpsFrames++;
        if (fpsTimer < fpsSampleSeconds) return;

        currentFps = Mathf.RoundToInt(fpsFrames / fpsTimer);
        haveFps = true;
        fpsTimer = 0f;
        fpsFrames = 0;
        RefreshHud();
    }

    void UpdateStressTest()
    {
        if (!stressRunning) return;
        if (!Session.CurrentState.RunsSimulation) return;   // only while Playing

        // Same rule as the sample: keep spawning while fps is above the target
        // (or until the minimum object count is reached), then stop and report.
        if (haveFps && currentFps <= targetFps && spawned.Count >= minObjects)
        {
            Debug.Log("Stress test stopped: " + spawned.Count + " objects at ~" + currentFps + " fps.");
            StopStressTest();
            return;
        }

        for (int i = 0; i < spawnPerFrame; i++) SpawnOne();
        RefreshHud();
    }

    void SpawnOne()
    {
        Vector3 pos = transform.position + (Vector3)(Random.insideUnitCircle * spawnRadius);
        //spawned.Add(Instantiate(stressPrefab, pos, Quaternion.identity));
        spawned.Add(Instantiate(stressPrefab, Random.insideUnitCircle * 8f, Quaternion.identity));
    }

    void RefreshHud()
    {
        if (numberOfObjectsViewer != null)
            numberOfObjectsViewer.SetText("{0}", spawned.Count);
        if (fpsCounter != null && haveFps)
            fpsCounter.SetText("fps: {0}", currentFps);
    }

    void OnDestroy()
    {
        StopStressTest();
        Time.timeScale = 1f;            // never leave the editor frozen
        AudioListener.pause = false;
        if (Session != null) Session.Registry.ShutdownAll();
        if (Instance == this) Instance = null;
    }

    // Quick test harness: remove or hide for real builds.
    void OnGUI()
    {
        if (!showDebugGui || Session == null) return;

        GUI.Label(new Rect(20, 20, 500, 25),
            Session.CurrentState.GetType().Name + "  |  features: " + Session.Registry.Count);

        if (GUI.Button(new Rect(20, 50, 160, 30), "Pause / Resume"))
            OnPausePressed();

        if (stressPrefab == null) return;

        if (GUI.Button(new Rect(20, 90, 160, 30), "Spawn 100"))
        {
            for (int i = 0; i < 100; i++) SpawnOne();
            RefreshHud();
        }

        if (GUI.Button(new Rect(20, 130, 160, 30), stressRunning ? "Stop stress test" : "Start stress test"))
        {
            if (stressRunning) StopStressTest(); else StartStressTest();
        }

        if (GUI.Button(new Rect(20, 170, 160, 30), "Clear spawned"))
            ClearSpawned();
    }
}