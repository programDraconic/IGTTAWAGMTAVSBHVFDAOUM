using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damage = 10;

    private StatModifier modifier = new PercentModifier();

    private WeaponBase weapon;

    private void Start()
    {
        weapon = new PistolWeapon(
            new WeaponData { weaponName = "Pistol", magazineSize = 12 },
            0.25f);
    }

    private void Update()
    {
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }

        if (Keyboard.current != null &&
            Keyboard.current.mKey.wasPressedThisFrame)
        {
            modifier = modifier is PercentModifier
                ? (StatModifier)new FlatModifier()
                : new PercentModifier();

            Debug.Log("Modifier: " + modifier.GetType().Name);
        }
    }

    private void Attack()
    {
        bool hadAmmo = weapon.HasAmmo;
        weapon.Fire();

        if (!hadAmmo)
            return;

        foreach (Collider2D hit in
                 Physics2D.OverlapCircleAll(transform.position, attackRange))
        {
            EnemyPlaceholder enemy = hit.GetComponent<EnemyPlaceholder>();

            if (enemy != null)
            {
                int finalDamage = Mathf.Max(
                1,
                Mathf.RoundToInt(modifier.Apply(damage)));

                Debug.Log($"Damage after {modifier.GetType().Name}: {finalDamage}");
                enemy.TakeDamage(finalDamage);
                break;
            }
        }
    }
}