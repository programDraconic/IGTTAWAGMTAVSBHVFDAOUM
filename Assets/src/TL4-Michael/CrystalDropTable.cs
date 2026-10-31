using System;
using UnityEngine;

// Create instances via Assets > Create > Roguelike > Crystal Drop Table.
// Each roll chooses ONE weighted entry, then spawns a random number of that crystal.
[CreateAssetMenu(fileName = "CrystalDropTable", menuName = "Roguelike/Crystal Drop Table")]
public class CrystalDropTable : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public CrystalPickup crystalPrefab;
        [Min(0)] public int weight = 1;
        [Min(0)] public int minCount = 1;
        [Min(0)] public int maxCount = 1;
    }

    [Range(0f, 1f)] public float dropChance = 1f;
    public Entry[] entries;

    public bool TryRoll(out CrystalPickup prefab, out int count)
    {
        prefab = null;
        count = 0;
        if (UnityEngine.Random.value > dropChance || entries == null) return false;

        int totalWeight = 0;
        foreach (Entry entry in entries)
        {
            if (entry != null && entry.crystalPrefab != null && entry.weight > 0)
                totalWeight += entry.weight;
        }
        if (totalWeight <= 0) return false;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        foreach (Entry entry in entries)
        {
            if (entry == null || entry.crystalPrefab == null || entry.weight <= 0) continue;
            roll -= entry.weight;
            if (roll >= 0) continue;

            prefab = entry.crystalPrefab;
            int lower = Mathf.Max(0, entry.minCount);
            int upper = Mathf.Max(lower, entry.maxCount);
            count = UnityEngine.Random.Range(lower, upper + 1);
            return count > 0;
        }
        return false;
    }
}
