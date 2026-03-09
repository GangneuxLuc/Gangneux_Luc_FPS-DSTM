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

    [Header("Detection Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float eyeHeight = 1.2f;
    [SerializeField] private float smellCheckInterval = 0.2f; // intervalle entre vérifs
    [SerializeField] private float smellTimer;

    private void Start()
    {
        timer = moosewanderInterval;
        isWandering = false;
        canSmellPlayer = false;
        smellTimer = 0f;
        agent.speed = mooseSpeed; // Assure que la vitesse est correctement appliquée à l'entrée dans WanderState
    }

    public override State RunCurrentState()
    {
        // Démarre la coroutine une seule fois
        if (!isWandering)
        {
            wanderCoroutine = StartCoroutine(WanderBehavior());
            isWandering = true;
        }

        // Si le joueur est senti, on stoppe les coroutines, on désactive ce State et on active l'AlertState
        if (canSmellPlayer)
        {
            // Stoppe la coroutine de wandering proprement
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

        if (!isWandering)
        {
            agent.ResetPath(); // stoppe le mouvement actuel pour éviter les comportements non voulus lors de la transition vers l'AlertState
        }
       
        wanderCoroutine = null;
    }

    Vector3 GetRandomNavMeshLocation(float radius)
    {
        Debug.Log("Finding random NavMesh location...");
        // Choisit une direction aléatoire dans une sphère de rayon défini
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        // Sample le NavMesh pour trouver le point valide le plus proche
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    void SmellZone()
    {
        float detectionRadius = mooseDetectionRadius * 0.5f;
        Vector3 center = transform.position + Vector3.up * eyeHeight;

        // Debug visuel 
        Debug.DrawLine(center, center + Vector3.up * detectionRadius, Color.magenta, 0.1f);

        // OverlapSphere détecte tous les colliders dans la sphère
        Collider[] hits = Physics.OverlapSphere(center, detectionRadius);
        foreach (var col in hits)
        {
            if (col == null) continue;

            GameObject hitObj = col.gameObject;
            // Supporte le tag sur l'objet touché ou sur la racine (child collider)
            if (hitObj.CompareTag(playerTag) || col.transform.root.gameObject.CompareTag(playerTag))
            {
                Debug.Log("Player detected in SmellZone (OverlapSphere) on: " + hitObj.name);
                canSmellPlayer = true;
                return;
            }
        }
    }

    private void Update()
    {
        // Appel périodique de la vérification pour économiser les performances
        smellTimer += Time.deltaTime;
        if (smellTimer >= smellCheckInterval)
        {
            smellTimer = 0f;
            SmellZone();
        }
    }


    private void OnDrawGizmos()
    {
        // Affiche la zone de détection dans l'éditeur (diamètre = mooseDetectionRadius)
        float detectionRadius = mooseDetectionRadius * 0.5f;
        Vector3 center = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRadius);
    }
}

