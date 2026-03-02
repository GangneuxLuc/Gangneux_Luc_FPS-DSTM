using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : State
{
    [Header("Chase State Settings")]
    public State wanderState;
    public float mooseChaseDuration = 7f;
    bool isChasing = false;
    bool isChaseOver = false;
    bool isPlayerDead = false;

    private void Start()
    {
        isChasing=true;
    }
    public override State RunCurrentState()
    {
        Debug.Log("Chasing the player...");
        

        if (isChasing)
        {
            StartCoroutine(ChaseBehavior());
        }

        if (isChaseOver)
        {
            isChasing = false;
            isChaseOver = false;
            return wanderState; // Transition vers l'état de patrouille après la poursuite
        }

        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDead = true;
        }
    }


    IEnumerator ChaseBehavior()
    {   
        agent.speed = mooseSpeed;
        agent.SetDestination(player.transform.position);
        Debug.Log("Moose is chasing the player...");
        if (player)
        yield return new WaitForSeconds(mooseChaseDuration); // Attendre un court délai pour éviter une mise à jour trop fréquente
        isChaseOver = true;
        yield break; 
    }
}
