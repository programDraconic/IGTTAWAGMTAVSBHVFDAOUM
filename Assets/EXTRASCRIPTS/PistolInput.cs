using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PistolInput : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponData weaponData = new WeaponData();

    [Tooltip("Minimum time between successful shots.")]
    [SerializeField, Min(0.01f)] private float burstDelay = 0.25f;

    [Header("Projectile Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField, Min(0f)] private float projectileSpeed = 15f;

    [Header("References")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform playerRoot;

    private PistolWeapon pistol;
    private bool isReloading;
    private bool fireInputReady;
    private Coroutine reloadRoutine;

    private void Awake()
    {
        pistol = new PistolWeapon(weaponData, burstDelay);

        if (muzzleFlash != null)
        {
            var main = muzzleFlash.main;
            main.playOnAwake = false;

            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    private void Update()
    {
        // Wait for a released mouse before accepting the first shot.
        if (!fireInputReady)
        {
            if (Mouse.current != null &&
                !Mouse.current.leftButton.isPressed &&
                !Mouse.current.leftButton.wasPressedThisFrame)
            {
                fireInputReady = true;
            }

            return;
        }

        if (isReloading)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (pistol.CurrentAmmo < weaponData.magazineSize)
            {
                reloadRoutine = StartCoroutine(ReloadRoutine());
                return;
            }
        }

        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (projectilePrefab == null || muzzle == null)
        {
            Debug.LogError(
                "Assign the projectile prefab and muzzle in the Inspector.",
                this
            );

            return;
        }

        Camera camera = Camera.main;

        if (camera == null)
        {
            Debug.LogError(
                "Assign the MainCamera tag to the gameplay camera.",
                this
            );

            return;
        }

        // Calculate aim before consuming ammunition.
        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        float distance = Mathf.Abs(
            camera.transform.position.z - muzzle.position.z
        );

        Vector3 mouseWorld = camera.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, distance)
        );

        Vector2 direction =
            (Vector2)mouseWorld - (Vector2)muzzle.position;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        if (!pistol.TryFire())
        {
            return;
        }

        ShootProjectile(direction.normalized);

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            muzzleFlash.Play(true);
        }
    }

    private void ShootProjectile(Vector2 direction)
    {
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        Projectile projectile = Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Transform owner = playerRoot != null
            ? playerRoot
            : transform.root;

        Collider2D[] bulletColliders =
            projectile.GetComponentsInChildren<Collider2D>();

        Collider2D[] playerColliders =
            owner.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D bulletCollider in bulletColliders)
        {
            foreach (Collider2D playerCollider in playerColliders)
            {
                Physics2D.IgnoreCollision(
                    bulletCollider,
                    playerCollider
                );
            }
        }

        projectile.Launch(
            direction,
            projectileSpeed,
            owner
        );
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        Debug.Log("Reloading...", this);

        yield return new WaitForSeconds(
            Mathf.Max(0f, weaponData.reloadTime)
        );

        pistol.Reload();

        isReloading = false;
        reloadRoutine = null;

        Debug.Log(
            $"Reload complete. Ammo: {pistol.CurrentAmmo}",
            this
        );
    }

    private void OnDisable()
    {
        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
        }

        isReloading = false;
        fireInputReady = false;
    }
}