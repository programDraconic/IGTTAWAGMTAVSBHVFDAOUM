using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSortController : MonoBehaviour
{
    private SortAlgorithm sorter = new SortAscending();

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            if (sorter is SortAscending)
                sorter = new SortDescending();
            else if (sorter is SortDescending)
                sorter = new SortAlgorithm();
            else
                sorter = new SortAscending();

            Debug.Log("Selected sorter: " + sorter.GetType().Name);
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
            Debug.Log(sorter.GetType().Name + " -> " + sorter.Sort());
    }
}