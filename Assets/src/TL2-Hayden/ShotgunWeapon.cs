using UnityEngine;

public class ShotgunWeapon : WeaponBase
{
    private int pelletCount;

    public ShotgunWeapon(WeaponData data, int pelletCount) : base(data)
    {
        this.pelletCount = pelletCount;
    }

    public override void Fire()
    {
        base.Fire();
        Debug.Log($"Shotgun fired {pelletCount} pellets.");
    }
}