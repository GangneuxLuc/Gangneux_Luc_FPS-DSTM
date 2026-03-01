using System.Collections;
using UnityEngine;

public class ChaseState : State
{
    [Header("Chase State Settings")]
    public State wanderState;
    public float mooseChaseDuration = 7f;
    bool mooseIsChasing = false;
    bool isChaseOver = false;
    public override State RunCurrentState()
    {
        Debug.Log("Chasing the player...");
        

        if (!mooseIsChasing)
        {
            StartCoroutine(ChaseBehavior());
        }

        if (isChaseOver)
        {
            mooseIsChasing = false;
            isChaseOver = false;
            return wanderState; // Transition vers l'état de patrouille après la poursuite
        }

        return this;
    }



    IEnumerator ChaseBehavior()
    {   
        mooseIsChasing = true;
        agent.speed = mooseSpeed;
        agent.SetDestination(player.transform.position);
        Debug.Log("Moose is chasing the player...");
        yield return new WaitForSeconds(mooseChaseDuration); // Attendre un court délai pour éviter une mise à jour trop fréquente
        isChaseOver = true;
        yield break; 
    }
}
