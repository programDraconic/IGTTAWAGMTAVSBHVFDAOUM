using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTests
{
    [Test]
    public void BaseReference_ToPistol_CallsPistolOverride()
    {
        WeaponBase weapon = new PistolWeapon(
            new WeaponData { weaponName = "Pistol", magazineSize = 12 },
            0.25f);

        LogAssert.Expect(LogType.Log, "Pistol fired. Ammo left: 11");
        LogAssert.Expect(LogType.Log, "Pistol burst delay: 0.25");

        weapon.Fire();
    }

    [Test]
    public void BaseReference_ToPistol_CanCallInheritedReload()
    {
        WeaponBase weapon = new PistolWeapon(
            new WeaponData { weaponName = "Pistol", magazineSize = 12 },
            0.25f);

        LogAssert.Expect(LogType.Log, "Pistol reloaded.");

        weapon.Reload();
    }
}