using UnityEngine;

public class Attack_State: Enemy_State
{
    
    public override string Tick()
    {
        return "enemy attack";
    }
}