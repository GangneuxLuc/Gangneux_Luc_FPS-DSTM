using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    [Header("Wander State Settings")]
    public AlertState alertState;
    public bool canSmellPlayer;
    private bool isWandering;
    private Coroutine wanderCoroutine;


  
    private void Start()
    {
        timer = moosewanderInterval;
        isWandering = false;
        canSmellPlayer = false;

    }


    public override State RunCurrentState()
    {
        // Démarre la coroutine une seule fois
        if (!isWandering)
        {
            wanderCoroutine = StartCoroutine(WanderBehavior());
            isWandering = true;
        }
        // Si le joueur est vu, on stoppe les coroutines et on change d'état
        if (canSmellPlayer)
        {
            if (wanderCoroutine != null)
            {
                StopCoroutine(wanderCoroutine);
                wanderCoroutine = null;
            }

            isWandering = false;
            canSmellPlayer = false;
            return alertState;
            

        }

        return this;
    }

    IEnumerator WanderBehavior()
    {
        while (!canSmellPlayer)
        {
            // Protection : si la condition change durant la boucle, on sort proprement
            if (canSmellPlayer)
            {
                break;
            }

            timer = timer + Time.deltaTime;
            if (timer >= moosewanderInterval)
            {
                Vector3 newPos = GetRandomNavMeshLocation(moosewanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }

            yield return null; // attend la frame suivante pour éviter une boucle serrée
        }

        // Nettoyage après arrêt de la coroutine
        isWandering = false;
        wanderCoroutine = null;
    }

    Vector3 GetRandomNavMeshLocation(float radius)
    {
        Debug.Log("Finding random NavMesh location...");
        // Pick a random direction
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        // Sample the NavMesh to find the nearest valid point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Fallback: stay in place if no valid point found
        return transform.position;
    }

   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected in WanderState!");
            canSmellPlayer = true;
        }
    }
   
}
