
using UnityEngine;

public class PlayerCrystals : MonoBehaviour
{
    [SerializeField] private int crystals = 0;

    public int CrystalCount => crystals;

    public void AddCrystals(int amount)
    {
        crystals += amount;

        Debug.Log($"Crystals collected: {crystals}");
    }
}
