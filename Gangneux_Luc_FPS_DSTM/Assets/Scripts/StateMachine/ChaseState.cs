using UnityEngine;

public class ChaseState : State
{
    public override State RunCurrentState()
    {
        Debug.Log("Chasing the player...");
        return this;
    }
}
