using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AlertState : State
{
    [Header("Alert State Settings")]
    public ChaseState chaseState;
    public bool isPlayerSpotted;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolInterval = 0.5f;
    [SerializeField] private float alertDuration = 8f;
    [SerializeField] private float eyeHeight = 1.2f;

    private Coroutine alertCoroutine;
    private Vector3 lastKnownPlayerPosition;
    private bool isChasing;

    public override State RunCurrentState()
    {
        // Si on vient d'être signalé, démarre le comportement d'alerte (patrouille)
        if (isPlayerSpotted)
        {
            isChasing = false;
            alertCoroutine = StartCoroutine(AlertBehavior());
        }

        // Si la coroutine a demandé la transition vers chase, on y va
        if (isChasing)
        {
            isChasing = false;
            return chaseState;
        }

        return this;
    }

    IEnumerator AlertBehavior()
    {

        while (!isChasing)
        {
            timer = timer + Time.deltaTime;
            if (timer >= moosewanderInterval)
            {
                Vector3 newPos = GetNearPlayerNavMeshLocation(moosewanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }

            yield return null; // attend la frame suivante pour éviter une boucle serrée
        }
    }

    Vector3 GetNearPlayerNavMeshLocation(float radius)
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerSpotted = true;
            Debug.Log("Player Spotted"); 
        }
    }
}
