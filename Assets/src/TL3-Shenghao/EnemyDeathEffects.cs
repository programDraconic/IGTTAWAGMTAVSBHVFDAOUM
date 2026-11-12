// Created by ChatGPT for the Shrimp Game project.
// Displays a temporary death sprite and plays a death sound.
// Connect PlayDeathEffects to EnemyHealth's On Death event.

using UnityEngine;

public class EnemyDeathEffects : MonoBehaviour
{
    [Header("Enemy Appearance")]
    [SerializeField] private SpriteRenderer enemyRenderer;

    [Header("Death Picture")]
    [SerializeField] private Sprite deathSprite;
    [SerializeField, Min(0.1f)] private float pictureDuration = 2f;
    [SerializeField, Min(0.01f)] private float pictureScale = 1f;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    private bool hasPlayed;

    private void Awake()
    {
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        hasPlayed = false;
    }

    public void PlayDeathEffects()
    {
        // Prevent repeated effects for the same death.
        if (hasPlayed)
        {
            return;
        }

        hasPlayed = true;

        CreateDeathPicture();
        PlayDeathSound();
    }

    private void CreateDeathPicture()
    {
        if (deathSprite == null || enemyRenderer == null)
        {
            return;
        }

        // Create an independent corpse with no collider or movement scripts.
        GameObject corpse = new GameObject("Enemy Death Picture");

        corpse.transform.SetPositionAndRotation(
            enemyRenderer.transform.position,
            enemyRenderer.transform.rotation
        );

        SpriteRenderer corpseRenderer =
            corpse.AddComponent<SpriteRenderer>();

        corpseRenderer.sprite = deathSprite;

        // Preserve the enemy's lighting material and drawing order.
        corpseRenderer.sharedMaterial = enemyRenderer.sharedMaterial;
        corpseRenderer.sortingLayerID = enemyRenderer.sortingLayerID;
        corpseRenderer.sortingOrder = enemyRenderer.sortingOrder;
        corpseRenderer.color = enemyRenderer.color;

        // Preserve horizontal facing. The death artwork is already belly-up.
        corpseRenderer.flipX = enemyRenderer.flipX;
        corpseRenderer.flipY = false;

        // Match the original sprite width despite different image sizes.
        float sizeRatio = 1f;

        if (enemyRenderer.sprite != null &&
            deathSprite.bounds.size.x > 0f)
        {
            sizeRatio =
                enemyRenderer.sprite.bounds.size.x /
                deathSprite.bounds.size.x;
        }

        corpse.transform.localScale =
            enemyRenderer.transform.lossyScale *
            sizeRatio *
            pictureScale;

        Destroy(corpse, pictureDuration);
    }

    private void PlayDeathSound()
    {
        if (deathSound == null)
        {
            return;
        }

        // An independent audio object keeps playing after the enemy is removed.
        GameObject soundObject = new GameObject("Enemy Death Sound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.clip = deathSound;
        source.Play();

        Destroy(soundObject, deathSound.length + 0.1f);
    }
}