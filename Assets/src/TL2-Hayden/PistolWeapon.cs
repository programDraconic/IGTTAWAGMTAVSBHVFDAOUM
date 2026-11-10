using UnityEngine;

public class PistolWeapon : WeaponBase
{
    private readonly float burstDelay;

    public PistolWeapon(
        WeaponData data,
        float burstDelay
    ) : base(data, burstDelay)
    {
        this.burstDelay = Cooldown;
    }

    protected override void OnFire()
    {
        Debug.Log($"Pistol burst delay: {burstDelay}");
    }
}