using UnityEngine;

public class BindingDemo : MonoBehaviour
{
    private WeaponBase[] weapons;
    private int currentIndex;

    private void Start()
    {
        weapons = new WeaponBase[]
        {
            new PistolWeapon(
                new WeaponData { weaponName = "Pistol", magazineSize = 12 },
                0.25f),

            new RifleWeapon(
                new WeaponData { weaponName = "Rifle", magazineSize = 30 },
                0.5f),

            new ShotgunWeapon(
                new WeaponData { weaponName = "Shotgun", magazineSize = 8 },
                8),

            new LauncherWeapon(
                new WeaponData { weaponName = "Launcher", magazineSize = 1 },
                null)
        };
    }

    private void OnGUI()
    {
        if (weapons == null)
            return;

        GUILayout.Label($"Current weapon: {weapons[currentIndex].GetType().Name}");

        if (GUILayout.Button("Fire"))
            weapons[currentIndex].Fire();

        if (GUILayout.Button("Swap weapon"))
            currentIndex = (currentIndex + 1) % weapons.Length;
    }
}