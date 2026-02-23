using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    public AlertState alertState;
    public bool canSeeThePlayer;

    private void Start()
    {
        Debug.Log("Entering WanderState...");
        timer = moosewanderInterval;

    }
    


    public override State RunCurrentState()
    {
            Debug.Log("Running WanderState...");
        if (canSeeThePlayer)
        {
                return alertState;
        }
        else 
        {
             return this;
        }
    }
    void Update()
    {
       
        timer = timer + Time.deltaTime;
        Debug.Log("On rentre dans l'update du WanderState");
        if (timer >= moosewanderInterval)
        {
            Debug.Log("Wandering...");
            Vector3 newPos = GetRandomNavMeshLocation(moosewanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
            Debug.Log("OK");
        }
       
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

}
