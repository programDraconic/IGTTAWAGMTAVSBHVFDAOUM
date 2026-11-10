using UnityEngine;

[System.Serializable]
public class WeaponData
{
    [Header("Weapon Information")]
    public string weaponName = "Weapon";

    [Header("Attack Settings")]
    [Min(1)]
    public int damage = 10;

    [Min(0f)]
    public float range = 5f;

    [Tooltip("Seconds between successful attacks.")]
    [Min(0.01f)]
    public float cooldown = 0.25f;

    [Header("Ammunition")]
    [Min(1)]
    public int magazineSize = 12;

    [Tooltip("Reload duration in seconds.")]
    [Min(0f)]
    public float reloadTime = 1.5f;
}