using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hit by object: '{other.gameObject.name}' with tag '{other.tag}'");

        // Check the object itself OR its root parent (handles child hitboxes/hurtboxes)
        if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
        {
            Debug.Log("Confirmed player - loading target scene.");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}