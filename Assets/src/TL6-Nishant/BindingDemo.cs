using UnityEngine;

public class BindingDemo : MonoBehaviour
{
    StatModifier current = new PercentModifier();

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), "Result: " + current.Apply(10f));
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is PercentModifier) ? new FlatModifier() : new PercentModifier();
    }
}
