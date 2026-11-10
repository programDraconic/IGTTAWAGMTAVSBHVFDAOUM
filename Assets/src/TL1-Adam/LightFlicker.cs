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


// AI-Generated Function 1 - Continuous frame-by-frame update - Lighting Management
    private void Update()
    {
        // Capture the time elapsed since the last frame to ensure frame-rate independent movement
        float dt = Time.deltaTime;
        
        // Controls the following features:
            // Light per-frame update.
            // Dropouts / blackouts (think: a faulty ceiling light that stops emitting light sporadically)
            // Flicker Modes - Random / Candle / Smooth

        // Dropout check: If the dropout timer is currently active, the light remains entirely off.
        if (dropoutTimer > 0f)
        {
            // Count down the remaining duration of the blackout using delta time
            dropoutTimer -= dt;
            
            // Force the light intensity to zero during a dropout
            SetLight(0f);
            
            // Skip the rest of the update loop for this frame
            return;
        }

        // Accumulate elapsed time toward the next scheduled intensity shift
        timer += dt;
        
        // Check if it is time to calculate a new target intensity or trigger a blackout
        if (timer >= nextChange)
        {
            // Reset the internal timer for the next interval cycle
            timer = 0f;
            
            // Call a helper method to determine how long until the next subsequent change occurs
            ScheduleNextChange();

            // Roll a random value to see if this change event triggers a sudden blackout (dropout)
            if (Random.value < dropoutChance)
            {
                // Select a random blackout duration from the defined vector range (min/max)
                dropoutTimer = Random.Range(dropoutDuration.x, dropoutDuration.y);
                return;
            }

            // If no dropout occurred, pick a new target intensity value within the allowed limits
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        // Evaluate the current lighting style mode using a switch statement
        switch (mode)
        {
            case FlickerMode.Random:
                // Instantly snap the current intensity directly to the target intensity
                currentIntensity = targetIntensity;
                break;

            case FlickerMode.Smooth:
                // Sample 1D Perlin noise over time using a unique seed and speed modifier for organic pulsation
                float noise = Mathf.PerlinNoise(seed, Time.time * smoothSpeed);
                // Map the 0-1 noise value smoothly between the minimum and maximum intensity limits
                currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
                break;

            case FlickerMode.Candle:
                // Calculate an exponential smoothing factor based on delta time to create flame-like sputtering
                float ease = 1f - Mathf.Exp(-candleSmoothing * dt);
                // Interpolate gradually from the current intensity toward the target intensity
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, ease);
                break;
        }

        // Apply the newly computed intensity value to the light source and its dependent properties
        SetLight(currentIntensity);
    }

    // AI-Generated Function 2 - Called every frame by update - Updates the Light Source
    private void SetLight(float intensity)
    {
        // Controls the following ambiance features:
            // Intensity
            // Color Shift
            // Radius Scaling (does the flicker happen only at the edge, or across the whole light span)
            // Normalized Radio - How does brightness dim across the span of the light?
            
        // Apply the raw intensity value directly to Unity's light component
        lampLight.intensity = intensity;

        // Calculate a normalized 0 to 1 ratio ('t') representing where the current intensity sits between limits.
        // 0 = dimmest, 1 = brightest. This is used to drive auxiliary effects like radius and color shifts.
        float t = Mathf.InverseLerp(minIntensity, maxIntensity, intensity);

        // If radius scaling is enabled, adjust the light's inner and outer bounds based on current brightness
        if (flickerRadius)
        {
            // Calculate a scale factor based on radius variation bounds and the normalized intensity 't'
            float scale = Mathf.Lerp(1f - radiusVariation, 1f + radiusVariation, t);
            
            // Apply the calculated scale to both outer and inner point light radii
            lampLight.pointLightOuterRadius = baseOuterRadius * scale;
            lampLight.pointLightInnerRadius = baseInnerRadius * scale;
        }

        // If color shifting is enabled, transition the light color between a dim state and a bright state
        if (flickerColor)
        {
            // Lerp the color smoothly based on the normalized intensity ratio 't'
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