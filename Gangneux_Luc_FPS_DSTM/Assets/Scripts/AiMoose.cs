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

    [Header("Moose Behavior Statuts")]
    private bool mooseIsAlerted;
    private bool mooseIsMoving;
    private bool mooseIsChasing;
    private bool mooseIsCalm;

    private NavMeshAgent agent;
    private float currentOrbitAngle;
   

   [Header("Wandering Settings")]
    private float timer;
    public float moosewanderRadius;
    public float moosewanderInterval;

    private void Awake() // Initialisation des valeurs
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
        else
        {
            // Assurer une vitesse par défaut correcte
            agent.speed = mooseSpeed > 0f ? mooseSpeed : agent.speed;
        }
    } 

    void Start()
    {
        mooseIsCalm = true; // Le moose commence dans un état calme
        mooseIsAlerted = false;
        mooseIsChasing = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        switch (mooseIsCalm, mooseIsAlerted, mooseIsChasing)
        {
            case (true, false, false):
                CalmBehavior();
                break;
            case (false, true, false):
                AlertBehavior();
                break;
            case (false, false, true):
                ChaseBehavior();
                break;
            default:
                CalmBehavior();
                break;
        }
    }



    void CalmBehavior() // Le comportement de base du moose, il erre tant que le joueur n'est pas détecté ou que sa santé mentale n'est pas trop basse
    {
        float pSanityLevel = player.GetComponent<Fps_Character>().PlayerSanityLevel;
        Debug.Log("Player Sanity Level: " + pSanityLevel);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && pSanityLevel > 60 && mooseIsCalm) // Si le moose n'est pas en train de se déplacer et que le joueur a une santé mentale suffisante, il wander
        {
          
        }
        else if (pSanityLevel < 60)
        {
            Debug.Log("Moose detected player with low sanity!");
            mooseIsAlerted = true;
            mooseIsCalm = false;
          
        }
    }

    void AlertBehavior() 
    {
        // Le moose se dirige vers le joueur à une vitesse réduite et fait son pathfinding dans une zone proche du joueur.

        if (agent == null || player == null) return;

        // Définir la vitesse d'alerte : utiliser mooseDetectionSpeed si disponible, sinon une fraction de la vitesse normale
        float alertSpeed = mooseDetectionSpeed > 0f ? mooseDetectionSpeed : mooseSpeed * 0.6f;
        agent.speed = alertSpeed;

        // Rayon autour du joueur dans lequel le moose choisira des points de navigation.
        // On utilise la moitié du rayon de détection comme zone proche, et on force un minimum pour éviter un rayon nul.
        float alertZoneRadius = Mathf.Max(3f, mooseDetectionRadius * 0.5f);

        // Choisir une position aléatoire sur le NavMesh autour du joueur et s'y diriger.
        Vector3 destinationNearPlayer = GetRandomNavMeshLocationAround(player.transform.position, alertZoneRadius);

        // Si la position trouvée est valide, on la définit comme destination.
        if (destinationNearPlayer != transform.position)
        {
            agent.SetDestination(destinationNearPlayer);
            mooseIsMoving = true;
        }
        else
        {
            // Fallback : regarde vers le joueur si aucune position NavMesh trouvée
            transform.LookAt(player.transform.position);
            mooseIsMoving = false;
        }

        // Optionnel : si le joueur se rapproche trop, passer en mode Chase (exemple simple)
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= agent.stoppingDistance + 1.5f)
        {
            mooseIsChasing = true;
            mooseIsAlerted = false;
            mooseIsCalm = false;
        }
    }

    void ChaseBehavior()
    {
      
        // Gardé pour extension : logique de poursuite si vous voulez qu'il chasse le joueur
    }


    // Nouvelle méthode : retourne une position valide sur le NavMesh autour d'un centre donné (ex: joueur)
    Vector3 GetRandomNavMeshLocationAround(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Fallback: retourne la position du centre si aucun point valide
        return center;
    }

    private void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mooseDetectionRadius);
        // Draw wandering radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, moosewanderRadius);
    }
       
}

