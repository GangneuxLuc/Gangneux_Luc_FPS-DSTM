using UnityEngine;
using UnityEngine.AI;

public class AiMoose : MonoBehaviour
{
    [SerializeField] private GameObject moose;
    [SerializeField] private GameObject player;
    



    [Header("Moose States and stats")]
    [SerializeField] private Moose_Data mooseData;
    private float mooseSpeed;
    private float mooseSpeedIncreaseRate;
    private float mooseSpeedDecreaseRate;
    private float mooseDetectionRadius;
    private float mooseDetectionSpeed;
    private float mooseOrbitRadius;


    [Header("Moose Behavior Statuts")]
    private bool mooseIsAlerted;
    private bool mooseIsMoving;
    private bool mooseIsChasing;
    private bool mooseIsCalm;

    private NavMeshAgent agent;
    private float currentOrbitAngle;
   

   [Header("Wandering Settings")]
    private float timer;
    public float moosewanderRadius = 10f;
    public float moosewanderInterval = 5f;

    private void Awake()
    {

    


        if (mooseData != null)
        {
            mooseSpeed = mooseData.speed;
            mooseSpeedIncreaseRate = mooseData.speedIncreaseRate;
            mooseSpeedDecreaseRate = mooseData.speedDecreaseRate;
            mooseDetectionRadius = mooseData.detectionRadius;
            mooseDetectionSpeed = mooseData.detectionSpeed;
            moosewanderRadius = mooseData.wanderRadius;
            moosewanderInterval = mooseData.wanderInterval;



           // agent.speed = mooseSpeed;


        }
        timer = moosewanderInterval; // Force le choix d'une nouvelle destination au démarrage
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("AiMoose: NavMeshAgent manquant sur le GameObject.");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        CalmBehavior();
    }

    void CalmBehavior()
    {
        float pSanityLevel = player.GetComponent<Fps_Character>().PlayerSanityLevel;
        Debug.Log("Player Sanity Level: " + pSanityLevel);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (timer >= moosewanderInterval)
            {
                Vector3 newPos = GetRandomNavMeshLocation(moosewanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        if (pSanityLevel < 60)
        {
            Debug.Log("Moose detected player with low sanity!");
            mooseIsAlerted = true;
            mooseIsCalm = false;
            AlertBehavior();
        }
    }

 
 




    void AlertBehavior()
    {
        // Gardé pour extension : comportements d'alerte supplémentaires si nécessaire
    }

    void ChaseBehavior()
    {
        // Gardé pour extension : logique de poursuite si vous voulez qu'il chasse le joueur
    }
    Vector3 GetRandomNavMeshLocation(float radius)
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 centre = moose != null ? moose.transform.position : transform.position;

        // Cercle extérieur et intérieur (min/max)
        Gizmos.DrawWireSphere(centre, mooseDetectionRadius);
        
        Vector3 orbitCentre = player != null ? player.transform.position : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(orbitCentre, mooseOrbitRadius);
    }
}

