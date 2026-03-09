using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AlertState : State
{
    public ChaseState chaseState;
    public bool canSeePlayer;
    private bool isPatrolling;
    private Coroutine alertCoroutine;

    public AudioClip mooseAlertClip;
    public AudioSource mooseAudioSource;

    [Header("Detection")]
    [SerializeField] private float eyeHeight = 1.2f;
    [SerializeField] private float fovAngle = 90f; // angle de vision devant le moose

    private void Start()
    {
        mooseAudioSource.PlayOneShot(mooseAlertClip); 
        moose.transform.LookAt(player.transform); // oriente le moose vers le joueur dès l'entrée dans AlertState
    }
    public override State RunCurrentState()
    {
        if (canSeePlayer)
        {
            if (alertCoroutine != null)
            {
                canSeePlayer = false; // reset pour éviter de rester bloqué en chase
                StopCoroutine(alertCoroutine);
                alertCoroutine = null;
            }
            return chaseState;
        }
        else
        {
            if (alertCoroutine == null)
            {
                alertCoroutine = StartCoroutine(AlertBehavior());
            }
            return this;
        }
    }

    IEnumerator AlertBehavior()
    {
        isPatrolling = true;

        // boucle principale : patrouille et détection chaque frame
        while (!canSeePlayer)
        {
            // Déplace l'agent vers une destination proche (si besoin)
            if (agent != null && (agent.pathPending == false && agent.remainingDistance <= agent.stoppingDistance))
            {
                Vector3 dest = GetNearPlayerNavMeshLocation(transform.position, moosewanderRadius);
                agent.SetDestination(dest);
                agent.speed = mooseSpeed * mooseDetectionSpeed; // augmente la vitesse pendant l'alert
            }

            // Vérification du joueur devant le moose
            PlayerDetection();

            yield return null;
        }

        isPatrolling = false;

        if (!isPatrolling)
        {
            agent.ResetPath(); // stoppe le mouvement actuel pour éviter les comportements non voulus lors de la transition vers le ChaseState
        }
        alertCoroutine = null;
    }

    // Renvoie une position Navigable proche du centre donné
    Vector3 GetNearPlayerNavMeshLocation(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    // Détecte si le joueur est devant le moose (angle + ligne de vue + distance)
    void PlayerDetection()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = player.transform.position - origin;
        float distanceToPlayer = toPlayer.magnitude;

        // on considère mooseDetectionRadius comme DIAMÈTRE -> radius = diameter / 2
        float detectionRadius = mooseDetectionRadius * 0.5f;
        if (distanceToPlayer > detectionRadius) return;

        Vector3 dir = toPlayer.normalized;

        // vérifie l'angle frontal
        float halfFov = fovAngle * 0.5f;
        if (Vector3.Angle(transform.forward, dir) > halfFov) return;

        // vérifie la ligne de vue (raycast)
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, detectionRadius))
        {
            if (hit.collider != null && (hit.collider.gameObject.CompareTag("Player") || hit.collider.transform.root.gameObject.CompareTag("Player")))
            {
                Debug.DrawRay(origin, dir * detectionRadius, Color.green, 0.2f);
                canSeePlayer = true;
                Debug.Log($"AlertState: canSeePlayer set TRUE on '{gameObject.name}' (id {GetInstanceID()})");
                return;
            }
        }

        Debug.DrawRay(origin, dir * detectionRadius, Color.red, 0.2f);
    }
}
