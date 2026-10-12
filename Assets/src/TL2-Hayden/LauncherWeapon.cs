using UnityEngine;

public class LauncherWeapon : WeaponBase
{
    private GameObject projectilePrefab;

    public LauncherWeapon(WeaponData data, GameObject projectilePrefab) : base(data)
    {
        this.projectilePrefab = projectilePrefab;
    }

    public override void Fire()
    {
        base.Fire();

        string projectileName = projectilePrefab != null
            ? projectilePrefab.name
            : "no projectile assigned";

        Debug.Log($"Launcher fired: {projectileName}");
    }
}