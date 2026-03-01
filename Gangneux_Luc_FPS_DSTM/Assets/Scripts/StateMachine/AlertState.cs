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
    private bool requestChase;

    public override State RunCurrentState()
    {
        // Si on vient d'être signalé, démarre le comportement d'alerte (patrouille)
        if (isPlayerSpotted && alertCoroutine == null)
        {
            isPlayerSpotted = false;
            requestChase = false;
            alertCoroutine = StartCoroutine(AlertBehavior());
        }

        // Si la coroutine a demandé la transition vers chase, on y va
        if (requestChase)
        {
            requestChase = false;
            return chaseState;
        }

        return this;
    }

    IEnumerator AlertBehavior()
    {
        // mémorise la position où le joueur a été senti
        if (player != null)
            lastKnownPlayerPosition = player.transform.position;

        float startTime = Time.time;

        Debug.Log($"AlertBehavior started around {lastKnownPlayerPosition}");

        while (Time.time - startTime < alertDuration)
        {
            // Choisit une destination aléatoire dans la zone d'alerte
            Vector3 dest = GetRandomNavMeshLocationAround(lastKnownPlayerPosition, patrolRadius);
            agent.SetDestination(dest);

            float waitTimer = 0f;
            // Attendre d'atteindre la destination ou un délai maximum
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                // Vérifie si l'agent voit le joueur pendant la patrouille
                if (player != null)
                {
                    Vector3 origin = transform.position + Vector3.up * eyeHeight;
                    Vector3 toPlayer = player.transform.position - origin;
                    float dist = toPlayer.magnitude;
                    Vector3 dir = toPlayer.normalized;

                    // Trace pour debug
                    Debug.DrawRay(origin, dir * Mathf.Min(dist, mooseDetectionRadius), Color.yellow);

                    RaycastHit hit;
                    if (Physics.Raycast(origin, dir, out hit, mooseDetectionRadius))
                    {
                        if (hit.collider != null && (hit.collider.gameObject.CompareTag("Player") || hit.collider.transform.root.gameObject.CompareTag("Player")))
                        {
                            Debug.Log("AlertState: player seen during patrol -> request chase");
                            requestChase = true;
                            // stop patrol immediately
                            alertCoroutine = null;
                            yield break;
                        }
                    }
                }

                // Timeout pour ne pas rester bloqué trop longtemps si agent ne peut atteindre la cible
                waitTimer += Time.deltaTime;
                if (waitTimer > 5f) break;

                yield return null;
            }

            // Pause courte entre destinations
            yield return new WaitForSeconds(patrolInterval);
        }

        Debug.Log("AlertBehavior finished without spotting player again.");
        alertCoroutine = null;
    }

    private Vector3 GetRandomNavMeshLocationAround(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // fallback : position actuelle
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerSpotted = true;
            lastKnownPlayerPosition = other.transform.position;
        }
    }
}
