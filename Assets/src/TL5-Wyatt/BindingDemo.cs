

using UnityEngine;

public class BindingDemo : MonoBehaviour
{

    SortAlgorithm current = new SortAscending();

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 600, 30),
                  current.GetType().Name + "  ->  " + current.Sort());

        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
        {
            if (current is SortAscending)       current = new SortDescending();
            else if (current is SortDescending) current = new SortAlgorithm();
            else                                current = new SortAscending();
        }
    }
}
