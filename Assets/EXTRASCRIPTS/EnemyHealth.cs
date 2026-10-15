using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hitsToKill = 3;

    private int hitsRemaining;

    private void Awake()
    {
        hitsRemaining = hitsToKill;
    }

    public void TakeHit()
    {
        hitsRemaining--;
        Debug.Log($"Enemy hit! {hitsRemaining} hits remaining.");

        if (hitsRemaining <= 0)
            Destroy(gameObject);
    }
}