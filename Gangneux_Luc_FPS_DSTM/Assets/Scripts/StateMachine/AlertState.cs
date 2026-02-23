using UnityEngine;

public class AlertState : State
{
    public ChaseState chaseState;
    public bool isPlayerSpotted;
    public override State RunCurrentState()
    {
        if (isPlayerSpotted)
        {
            return chaseState;
        }
        else
        {
            return this;
        }
           
    }
}
