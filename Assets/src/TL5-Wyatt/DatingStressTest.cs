using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatingStressTest : MonoBehaviour
{
    [SerializeField] private Vector2 arenaHalfSize = new Vector2(8f, 4.5f);
    [SerializeField, Min(1)] private int startingObjects = 8;
    [SerializeField, Min(1)] private int maximumObjects = 8192;
    [SerializeField, Min(0.25f)] private float settleSeconds = 0.5f;
    [SerializeField, Min(0.25f)] private float sampleSeconds = 1f;

    private DatingController dating;
    private readonly List<GiftStressBody> bodies = new List<GiftStressBody>();

    private float fps;
    private float elapsed;
    private int frames;
    private float sampledFps;
    private string status = "Sampling starting FPS";
    private long giftsProcessed;
    private bool finished;

    public int StressPoint { get { return bodies.Count; } }
    public float StartingFps { get; private set; }
    public float HalfFps { get { return StartingFps * 0.5f; } }
    public int FpsBreakPoint { get; private set; }
    public int PhysicsBreakPoint { get; private set; }
    public string PhysicsBreakDetail { get; private set; }

    private void Start()
    {
        PhysicsBreakDetail = "";
        dating = new DatingController();
        dating.EnterDatingPhase();
        StartCoroutine(Run());
    }

    private void Update()
    {
        frames++;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed >= 0.5f)
        {
            fps = frames / elapsed;
            frames = 0;
            elapsed = 0f;
        }
    }

    public void ApplyGift(int suitorIndex)
    {
        dating.SelectSuitor(suitorIndex);
        dating.OfferGift(Gift.ShrimpCracker);
        giftsProcessed++;
    }

    public void ReportPhysicsViolation(string bodyName, Vector2 position)
    {
        if (PhysicsBreakPoint != 0) return;
        PhysicsBreakPoint = bodies.Count;
        PhysicsBreakDetail = bodyName + " escaped the arena at (" +
                             position.x.ToString("F1") + ", " + position.y.ToString("F1") + ")";
        Debug.LogWarning("[TL5 stress] PHYSICS BREAK at " + PhysicsBreakPoint + " objects: " + PhysicsBreakDetail);
    }

    private IEnumerator Run()
    {
        yield return new WaitForSecondsRealtime(settleSeconds);
        Spawn(startingObjects);
        yield return new WaitForSecondsRealtime(settleSeconds);
        yield return Measure();
        StartingFps = sampledFps;

        while (bodies.Count < maximumObjects)
        {
            if (FpsBreakPoint != 0 && PhysicsBreakPoint != 0) break;

            status = "Escalating";
            int target = Mathf.Min(maximumObjects, Mathf.Max(1, bodies.Count * 2));
            Spawn(target - bodies.Count);

            yield return new WaitForSecondsRealtime(settleSeconds);
            yield return Measure();

            if (FpsBreakPoint == 0 && sampledFps <= HalfFps)
            {
                FpsBreakPoint = bodies.Count;
                Debug.LogWarning("[TL5 stress] FPS BREAK at " + FpsBreakPoint + " objects: " +
                                 sampledFps.ToString("F1") + " FPS, half of " + StartingFps.ToString("F1"));
            }
        }

        Finish();
    }

    private IEnumerator Measure()
    {
        status = "Measuring at " + bodies.Count + " objects";
        int firstFrame = Time.frameCount;
        float firstTime = Time.realtimeSinceStartup;
        yield return new WaitForSecondsRealtime(sampleSeconds);
        sampledFps = (Time.frameCount - firstFrame) / (Time.realtimeSinceStartup - firstTime);
    }

    private void Spawn(int count)
    {
        for (int i = 0; i < count && bodies.Count < maximumObjects; i++)
        {
            GameObject go = new GameObject("Gift " + bodies.Count);
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(
                Random.Range(-arenaHalfSize.x + 0.5f, arenaHalfSize.x - 0.5f),
                Random.Range(-arenaHalfSize.y + 0.5f, arenaHalfSize.y - 0.5f), 0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GiftStressBody.SharedSprite;
            sr.color = Color.HSVToRGB((bodies.Count % 4) * 0.25f, 0.55f, 1f);

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            go.AddComponent<CircleCollider2D>().radius = 0.12f;

            GiftStressBody gsb = go.AddComponent<GiftStressBody>();
            gsb.Initialise(this, bodies.Count % 4, arenaHalfSize, Random.insideUnitCircle);
            bodies.Add(gsb);
        }
    }

    private void Finish()
    {
        if (finished) return;
        finished = true;
        status = "Finished";

        Debug.Log("[TL5 stress] RESULT" +
                  "  starting FPS " + StartingFps.ToString("F1") +
                  " | half " + HalfFps.ToString("F1") +
                  " | FPS break " + (FpsBreakPoint == 0 ? "not reached" : FpsBreakPoint + " objects") +
                  " | physics break " + (PhysicsBreakPoint == 0 ? "not reached" : PhysicsBreakPoint + " objects") +
                  " | gifts processed " + giftsProcessed +
                  " | HARDWARE " + Hardware());
    }

    private string Hardware()
    {
        return SystemInfo.deviceModel + " / " + SystemInfo.processorType + " / " +
               SystemInfo.graphicsDeviceName + " / " + Application.platform;
    }

    private void OnGUI()
    {
        float w = Mathf.Min(520f, Screen.width - 24f);
        GUILayout.BeginArea(new Rect(12f, 12f, w, 250f), GUI.skin.box);
        GUILayout.Label("TL5 / F6 Dating - Stress Scene");
        GUILayout.Label("Current Stress Point:  " + StressPoint + " objects");
        GUILayout.Label("Current FPS:           " + fps.ToString("F1"));
        GUILayout.Label("Starting FPS:          " + (StartingFps <= 0f ? "sampling..." : StartingFps.ToString("F1") +
                        "   (break at " + HalfFps.ToString("F1") + ")"));
        GUILayout.Label("FPS Break Point:       " + (FpsBreakPoint == 0 ? "-" : FpsBreakPoint + " objects"));
        GUILayout.Label("Physics Break Point:   " + (PhysicsBreakPoint == 0 ? "-" : PhysicsBreakPoint + " objects"));
        if (PhysicsBreakPoint != 0) GUILayout.Label("   " + PhysicsBreakDetail);
        GUILayout.Label("Gifts processed:       " + giftsProcessed);
        GUILayout.Label(status);
        GUILayout.Label("Hardware: " + Hardware());
        GUILayout.EndArea();
    }
}
