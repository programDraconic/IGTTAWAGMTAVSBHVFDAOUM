using UnityEngine;

public class RifleWeapon : WeaponBase
{
    private float recoilAmount;

    public RifleWeapon(WeaponData data, float recoilAmount) : base(data)
    {
        this.recoilAmount = recoilAmount;
    }

    public override void Fire()
    {
        base.Fire();
        Debug.Log($"Rifle recoil: {recoilAmount}");
    }
}