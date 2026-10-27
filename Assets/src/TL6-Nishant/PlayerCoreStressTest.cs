using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TL6
{
    public class PlayerCoreStressTest : MonoBehaviour
    {
        [SerializeField] private PlayerCoreStressBody prefab;
        [SerializeField] private Vector2 arenaHalfSize = new Vector2(8f, 4f);
        [SerializeField, Min(1)] private int maximumObjects = 2000;
        [SerializeField, Min(0.5f)] private float sampleSeconds = 2f;
        [SerializeField, Min(0.1f)] private float settleSeconds = 1f;

        private readonly List<PlayerCoreStressBody> instances = new List<PlayerCoreStressBody>();
        private float fps;
        private float elapsed;
        private int frames;
        private bool usePercent = true;
        private Coroutine stressRun;
        private float sampledFps;
        private string message = "Ready";

        public int InstanceCount => instances.Count;
        public bool IsRunning => stressRun != null;
        public float StartingFps { get; private set; }
        public float StopAtFps => StartingFps / 2f;
        public int FpsBreakCount { get; private set; }

        private void Start()
        {
            // An exported phone/laptop build runs without needing controls.
            if (Application.isEditor) Spawn(1);
            else StartTest();
        }

        private void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            frames++;
            if (elapsed < 0.5f) return;
            fps = frames / elapsed;
            frames = 0;
            elapsed = 0f;
        }

        public void Spawn(int count)
        {
            if (prefab == null) return;

            int available = Mathf.Max(0, maximumObjects - instances.Count);
            for (int i = 0; i < Mathf.Clamp(count, 0, available); i++)
            {
                Vector2 position = new Vector2(
                    Random.Range(-arenaHalfSize.x + 0.3f, arenaHalfSize.x - 0.3f),
                    Random.Range(-arenaHalfSize.y + 0.3f, arenaHalfSize.y - 0.3f));
                if (instances.Count == 0) position = new Vector2(0f, -arenaHalfSize.y / 2f);
                PlayerCoreStressBody instance = Instantiate(prefab, position, Quaternion.identity, transform);
                instance.Initialise(Random.insideUnitCircle, arenaHalfSize);
                instance.UsePercentModifier(usePercent);
                instances.Add(instance);
            }
        }

        public void Clear()
        {
            StopTest();
            foreach (PlayerCoreStressBody instance in instances)
                if (instance != null) Destroy(instance.gameObject);
            instances.Clear();
            StartingFps = 0f;
            FpsBreakCount = 0;
            message = "Ready";
        }

        public void StartTest()
        {
            if (prefab == null) return;
            Clear();
            Spawn(1);
            stressRun = StartCoroutine(RunTest());
        }

        public void StopTest()
        {
            if (stressRun != null) StopCoroutine(stressRun);
            stressRun = null;
            message = "Stopped";
        }

        private IEnumerator RunTest()
        {
            message = "Measuring starting FPS...";
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, settleSeconds));
            yield return MeasureFps();
            StartingFps = sampledFps;

            while (InstanceCount < maximumObjects)
            {
                Spawn(100);
                message = "Adding objects and measuring...";
                yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, settleSeconds));
                yield return MeasureFps();

                if (sampledFps <= StopAtFps)
                {
                    FpsBreakCount = InstanceCount;
                    message = $"Break: {FpsBreakCount} objects, {sampledFps:F1} FPS";
                    FinishTest();
                    yield break;
                }
            }

            message = "Limit reached; FPS not halved.";
            FinishTest();
        }

        private IEnumerator MeasureFps()
        {
            int firstFrame = Time.frameCount;
            float firstTime = Time.realtimeSinceStartup;
            yield return new WaitForSecondsRealtime(Mathf.Max(0.5f, sampleSeconds));
            sampledFps = (Time.frameCount - firstFrame) / (Time.realtimeSinceStartup - firstTime);
        }

        private void FinishTest()
        {
            stressRun = null;
            Debug.Log($"Stress test: {message} Starting {StartingFps:F1} FPS, stop at {StopAtFps:F1}, measured {sampledFps:F1}. Device: {SystemInfo.deviceModel} ({Application.platform}).", this);
        }

        private void OnDisable()
        {
            if (IsRunning) StopTest();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12, 12, Mathf.Min(300, Screen.width - 24), 230), GUI.skin.box);
            GUILayout.Label("Stress Test");
            GUILayout.Label($"Objects: {InstanceCount}   FPS: {fps:F1}");
            if (StartingFps > 0f)
                GUILayout.Label($"Starting: {StartingFps:F1}   Half: {StopAtFps:F1} FPS");
            GUILayout.Label(message);
            GUILayout.Label($"Might from base 10: {(usePercent ? "12 (percent)" : "15 (flat)")}");
            if (prefab == null) GUILayout.Label("Assign the player prefab in the Inspector.");
            if (GUILayout.Button(IsRunning ? "Stop test" : "Start test"))
            {
                if (IsRunning) StopTest();
                else StartTest();
            }
            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = previouslyEnabled && !IsRunning;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spawn 10")) Spawn(10);
            if (GUILayout.Button("Spawn 100")) Spawn(100);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Switch modifier"))
            {
                usePercent = !usePercent;
                foreach (PlayerCoreStressBody instance in instances)
                    if (instance != null) instance.UsePercentModifier(usePercent);
            }
            GUI.enabled = previouslyEnabled;
            if (GUILayout.Button("Clear all")) Clear();
            GUILayout.EndArea();
        }
    }
}
