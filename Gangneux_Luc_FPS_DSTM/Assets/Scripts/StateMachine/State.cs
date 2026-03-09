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

    [Header("References")]
    [SerializeField] protected GameObject player;
    protected NavMeshAgent agent;
    protected gameDirector gameDirector;

    [Header("Wandering Settings")]
    public float timer;
    public float moosewanderRadius;
    public float moosewanderInterval;

    [Header("Game Over")]
    public bool isPlayerDead = false;
    protected virtual void Awake()
    {
        agent = moose.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameDirector = GameObject.FindAnyObjectByType<gameDirector>();

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
            agent.speed = mooseSpeed;
        }
    }

}
