using UnityEngine;

public abstract class WeaponBase
{
    protected WeaponData weaponData;
    protected int currentAmmo;
    public bool HasAmmo => currentAmmo > 0;

    protected WeaponBase(WeaponData data)
    {
        weaponData = data;
        currentAmmo = data.magazineSize;
    }

    public virtual void Fire()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log($"{weaponData.weaponName} is out of ammo.");
            return;
        }

        currentAmmo--;
        Debug.Log($"{weaponData.weaponName} fired. Ammo left: {currentAmmo}");
    }

    public virtual void Reload()
    {
        currentAmmo = weaponData.magazineSize;
        Debug.Log($"{weaponData.weaponName} reloaded.");
    }
}