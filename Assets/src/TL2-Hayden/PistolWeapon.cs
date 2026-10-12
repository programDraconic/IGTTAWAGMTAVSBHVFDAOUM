using UnityEngine;

public class PistolWeapon : WeaponBase
{
    private float burstDelay;

    public PistolWeapon(WeaponData data, float burstDelay) : base(data)
    {
        this.burstDelay = burstDelay;
    }

    public override void Fire()
    {
        base.Fire();
        Debug.Log($"Pistol burst delay: {burstDelay}");
    }
}