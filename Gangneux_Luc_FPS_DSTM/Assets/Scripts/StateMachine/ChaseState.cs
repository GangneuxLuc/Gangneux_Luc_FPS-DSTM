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
    private Coroutine chaseCoroutine;

    private void Start()
    {
        // Ne pas démarrer la coroutine ici sans garde
        isChasing = true; // déclenche la logique de chase, la coroutine ne sera démarrée qu'une fois
    }

    public override State RunCurrentState()
    {
        // démarre la coroutine une seule fois lorsqu'on est en train de chasser
        if (isChasing && chaseCoroutine == null)
        {
            chaseCoroutine = StartCoroutine(ChaseBehavior());
        }
        if (isChaseOver)
        {
            Debug.Log("ChaseState: Chase is over. Transitioning to WanderState...");
            StopCoroutine(chaseCoroutine);
            isChasing = false;
            isChaseOver = false;
            return wanderState;
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
        // garde une référence locale de l'agent et du joueur
        if (agent == null)
        {
            Debug.LogError("ChaseState: NavMeshAgent is null");
            yield break;
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("ChaseState: player not found");
                yield break;
            }
        }

        agent.speed = mooseSpeed * 3f;

        float startTime = Time.time;
        while (Time.time - startTime < mooseChaseDuration)
        {
            // met à jour la destination pendant la poursuite
            agent.SetDestination(player.transform.position);
            yield return null;
        }

        isChaseOver = true;
        chaseCoroutine = null;

        if (isChaseOver)
        {
            agent.speed = mooseSpeed; // réinitialise la vitesse du moose après la poursuite
            agent.ResetPath(); // stoppe le mouvement actuel pour éviter les comportements non voulus lors de la transition vers le WanderState
        }

        yield break;
    }

 
}
