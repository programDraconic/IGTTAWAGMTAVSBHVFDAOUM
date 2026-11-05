using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    public enum FlickerMode
    {
        [Tooltip("Snaps to a new random intensity at each interval. Harsh, like a failing bulb.")]
        Random,
        [Tooltip("Smooth Perlin noise that drifts continuously. Good for a steady, uneasy glow.")]
        Smooth,
        [Tooltip("Random targets that ease toward each other. Looks like a flame.")]
        Candle
    }

    [Header("Intensity")]
    [SerializeField] private FlickerMode mode = FlickerMode.Candle;
    [SerializeField, Min(0f)] private float minIntensity = 0.2f;
    [SerializeField, Min(0f)] private float maxIntensity = 1.5f;

    [Header("Timing")]
    [Tooltip("Random time between target changes (Random and Candle modes).")]
    [SerializeField, Min(0.001f)] private float minInterval = 0.05f;
    [SerializeField, Min(0.001f)] private float maxInterval = 0.15f;
    [Tooltip("How quickly Candle mode eases toward its target. Higher is snappier.")]
    [SerializeField, Min(0f)] private float candleSmoothing = 12f;
    [Tooltip("Speed of the noise in Smooth mode.")]
    [SerializeField, Min(0f)] private float smoothSpeed = 3f;

    [Header("Dropouts")]
    [Tooltip("Chance each change that the light briefly goes out.")]
    [SerializeField, Range(0f, 1f)] private float dropoutChance = 0.02f;
    [Tooltip("Min and max length of a dropout in seconds.")]
    [SerializeField] private Vector2 dropoutDuration = new Vector2(0.05f, 0.3f);

    [Header("Radius (optional)")]
    [Tooltip("Shrink the light's radius when it dims, grow it when it brightens. Point lights only.")]
    [SerializeField] private bool flickerRadius = false;
    [Tooltip("How much the radius changes, as a fraction of its base size (0.2 = 20%).")]
    [SerializeField, Range(0f, 1f)] private float radiusVariation = 0.15f;

    [Header("Color (optional)")]
    [SerializeField] private bool flickerColor = false;
    [SerializeField] private Color dimColor = new Color(1f, 0.45f, 0.15f);
    [SerializeField] private Color brightColor = new Color(1f, 0.75f, 0.4f);

    private Light2D lampLight;
    private float baseOuterRadius;
    private float baseInnerRadius;
    private float seed;
    private float timer;
    private float nextChange;
    private float dropoutTimer;
    private float targetIntensity;
    private float currentIntensity;

    private void Awake()
    {
        lampLight = GetComponent<Light2D>();
        baseOuterRadius = lampLight.pointLightOuterRadius;
        baseInnerRadius = lampLight.pointLightInnerRadius;
        seed = Random.Range(0f, 1000f);
        currentIntensity = maxIntensity;
        targetIntensity = maxIntensity;
        ScheduleNextChange();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Dropout: the light is off until the timer runs out.
        if (dropoutTimer > 0f)
        {
            dropoutTimer -= dt;
            SetLight(0f);
            return;
        }

        timer += dt;
        if (timer >= nextChange)
        {
            timer = 0f;
            ScheduleNextChange();

            if (Random.value < dropoutChance)
            {
                dropoutTimer = Random.Range(dropoutDuration.x, dropoutDuration.y);
                return;
            }

            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        switch (mode)
        {
            case FlickerMode.Random:
                currentIntensity = targetIntensity;
                break;

            case FlickerMode.Smooth:
                float noise = Mathf.PerlinNoise(seed, Time.time * smoothSpeed);
                currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
                break;

            case FlickerMode.Candle:
                float ease = 1f - Mathf.Exp(-candleSmoothing * dt);
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, ease);
                break;
        }

        SetLight(currentIntensity);
    }

    private void SetLight(float intensity)
    {
        lampLight.intensity = intensity;

        // 0 = dimmest, 1 = brightest. Used to drive radius and color.
        float t = Mathf.InverseLerp(minIntensity, maxIntensity, intensity);

        if (flickerRadius)
        {
            float scale = Mathf.Lerp(1f - radiusVariation, 1f + radiusVariation, t);
            lampLight.pointLightOuterRadius = baseOuterRadius * scale;
            lampLight.pointLightInnerRadius = baseInnerRadius * scale;
        }

        if (flickerColor)
        {
            lampLight.color = Color.Lerp(dimColor, brightColor, t);
        }
    }

    private void ScheduleNextChange()
    {
        nextChange = Random.Range(minInterval, maxInterval);
    }

    private void OnValidate()
    {
        // Keep the ranges sane when edited in the Inspector.
        if (maxIntensity < minIntensity) maxIntensity = minIntensity;
        if (maxInterval < minInterval) maxInterval = minInterval;
        if (dropoutDuration.y < dropoutDuration.x) dropoutDuration.y = dropoutDuration.x;
    }
}