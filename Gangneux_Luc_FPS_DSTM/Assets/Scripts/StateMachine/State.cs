using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    public abstract State RunCurrentState();
    
   

    [Header("Moose States and stats")]
    [SerializeField] protected GameObject moose;
    [SerializeField] protected Moose_Data mooseData;
    protected float mooseSpeed;
    protected float mooseSpeedIncreaseRate;
    protected float mooseSpeedDecreaseRate;
    protected float mooseDetectionRadius;
    protected float mooseDetectionSpeed;

    [Header("Moose Behavior Statuts")]
    protected bool mooseIsAlerted;
    protected bool mooseIsMoving;
    protected bool mooseIsChasing;
    protected bool mooseIsCalm;

    protected NavMeshAgent agent;
    protected float currentOrbitAngle;


    [Header("Wandering Settings")]
    public float timer;
    public float moosewanderRadius;
    public float moosewanderInterval;
    protected virtual void Awake()
    {
        agent = moose.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("State: NavMeshAgent manquant sur le GameObject.");
        }
        timer = moosewanderInterval;
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


    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
