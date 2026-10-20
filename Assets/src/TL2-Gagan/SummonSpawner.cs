using UnityEngine;

public class SummonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject summonPrefab;

    private GameObject spawnedSummon;

    public void SpawnSummon()
    {
        if (summonPrefab == null)
        {
            Debug.LogWarning("Gagan summon prefab is not assigned.");
            return;
        }

        if (spawnedSummon != null)
        {
            Destroy(spawnedSummon);
        }

        Vector3 spawnPosition =
            transform.position + new Vector3(2f, 0f, 0f);

        spawnedSummon = Instantiate(
            summonPrefab,
            spawnPosition,
            Quaternion.identity);

        Summon summon = new OrbitingSummon();
        summon.Activate();
        summon.UpdateBehavior(
            new SummonContext((Vector2)transform.position));

        Debug.Log(
            "Spawned " + summon.GetType().Name +
            ": " + summon.LastBehavior);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(Screen.width - 230, 20, 210, 70),
            GUI.skin.box);

        if (GUILayout.Button("Spawn Gagan Summon"))
        {
            SpawnSummon();
        }

        GUILayout.EndArea();
    }
}
