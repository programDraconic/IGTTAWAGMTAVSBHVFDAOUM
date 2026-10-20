using UnityEngine;

public class EnemyPlaceholder : MonoBehaviour
{
    [SerializeField] private int health = 40;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Enemy hit! Health: {health}");

        if (health <= 0)
            Destroy(gameObject);
    }
}