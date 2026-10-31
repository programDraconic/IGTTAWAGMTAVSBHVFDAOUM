using System;
using UnityEngine;

public abstract class WeaponBase
{
    protected WeaponData weaponData;
    protected int currentAmmo;

    private readonly float cooldown;
    private float nextFireTime = float.NegativeInfinity;

    public bool HasAmmo => currentAmmo > 0;
    public int CurrentAmmo => currentAmmo;
    public int Damage => weaponData.damage;
    public float Range => weaponData.range;
    public float Cooldown => cooldown;

    protected WeaponBase(
        WeaponData data,
        float? cooldownOverride = null)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        weaponData = data;
        currentAmmo = Mathf.Max(0, data.magazineSize);

        cooldown = Mathf.Max(
            0.01f,
            cooldownOverride ?? data.cooldown
        );
    }

    public virtual void Fire()
    {
        TryFire();
    }

    public bool TryFire()
    {
        return TryFire(Time.time);
    }

    // Explicit time allows tests to check cooldowns without waiting.
    public bool TryFire(float currentTime)
    {
        if (float.IsNaN(currentTime) ||
            float.IsInfinity(currentTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentTime)
            );
        }

        if (!HasAmmo)
        {
            return false;
        }

        if (currentTime < nextFireTime)
        {
            return false;
        }

        currentAmmo--;
        nextFireTime = currentTime + cooldown;

        Debug.Log(
            $"{weaponData.weaponName} fired. " +
            $"Ammo left: {currentAmmo}"
        );

        OnFire();

        return true;
    }

    protected virtual void OnFire()
    {
    }

    public virtual void Reload()
    {
        currentAmmo = Mathf.Max(0, weaponData.magazineSize);

        Debug.Log($"{weaponData.weaponName} reloaded.");
    }
}