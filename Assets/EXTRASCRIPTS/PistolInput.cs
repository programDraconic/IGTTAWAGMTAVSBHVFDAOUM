using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PistolInput : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData = new WeaponData();
    [SerializeField] private float burstDelay = 0.25f;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform playerRoot;

    private PistolWeapon pistol;
    private bool isReloading;
    private Coroutine reloadRoutine;

    private void Awake()
    {
        pistol = new PistolWeapon(weaponData, burstDelay);
    }

    private void Update()
    {
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
            "Assign the projectile prefab and muzzle in the Inspector."
        );

        return;
    }

    if (pistol.TryFire())
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            muzzleFlash.Play(true);
        }

        ShootProjectile();
    }
}

private IEnumerator ReloadRoutine()
{
    isReloading = true;

    Debug.Log("Reloading...");

    yield return new WaitForSeconds(
        Mathf.Max(0f, weaponData.reloadTime)
    );

    pistol.Reload();

    isReloading = false;
    reloadRoutine = null;

    Debug.Log($"Reload complete. Ammo: {pistol.CurrentAmmo}");
}

private void OnDisable()
{
    if (reloadRoutine != null)
    {
        StopCoroutine(reloadRoutine);
        reloadRoutine = null;
    }

    isReloading = false;
}

    private void ShootProjectile()
    {
        Vector2 direction = muzzle.right.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Projectile projectile = Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Collider2D bulletCollider = projectile.GetComponent<Collider2D>();
        Collider2D[] playerColliders = transform.root.GetComponentsInChildren<Collider2D>();

        if (bulletCollider != null)
        {
            foreach (Collider2D playerCollider in playerColliders)
                Physics2D.IgnoreCollision(bulletCollider, playerCollider);
        }

        projectile.Launch(direction, projectileSpeed, playerRoot);
    }
}