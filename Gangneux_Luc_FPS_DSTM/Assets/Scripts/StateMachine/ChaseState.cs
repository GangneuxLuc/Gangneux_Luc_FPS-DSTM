using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : State
{
    [Header("Chase State Settings")]
    public State wanderState;
    public float mooseChaseDuration = 7f;

    private bool isChaseOver = false;
    private Coroutine chaseCoroutine;

    private void OnEnable()
    {
        // Réinitialiser l'état à chaque activation du State pour garantir
        // que la coroutine démarre correctement quand on entre en Chase.
        isChaseOver = false;
    }

    public override State RunCurrentState()
    {
        // démarre la coroutine une seule fois lorsqu'on entre dans l'état
        if (chaseCoroutine == null)
        {
            chaseCoroutine = StartCoroutine(ChaseBehavior());
        }

        if (isChaseOver)
        {
            Debug.Log("ChaseState: Chase is over. Transitioning to WanderState...");
            // nettoyage : ne jamais StopCoroutine(null)
            if (chaseCoroutine != null)
            {
                StopCoroutine(chaseCoroutine);
                chaseCoroutine = null;
            }
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

    private void OnDisable()
    {
        // Arrêter proprement la coroutine si l'état est désactivé
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
    }

    IEnumerator ChaseBehavior()
    {
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

        
        Debug.Log("ChaseState: Starting chase behavior. Moose speed set to " + agent.speed);

        float startTime = Time.time;
        while (Time.time - startTime < mooseChaseDuration)
        {
            agent.speed = mooseSpeed * 3f;
            agent.SetDestination(player.transform.position);
            yield return null;
        }

        // Fin normale de la coroutine
        agent.speed = 2f; // réinitialise la vitesse
        agent.ResetPath(); // stoppe le mouvement actuel pour une transition propre
        isChaseOver = true;
        chaseCoroutine = null;
        yield break;
    }
}
