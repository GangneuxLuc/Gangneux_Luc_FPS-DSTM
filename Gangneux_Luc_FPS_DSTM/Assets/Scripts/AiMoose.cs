using UnityEngine;
using UnityEngine.AI;

public class AiMoose : MonoBehaviour
{
    [SerializeField] private Fps_Character player;
    [SerializeField] private GameObject moose;


    public Transform mooseGoal;
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

    private void Awake()
    {
        if (mooseData == null)
        {
            mooseSpeed = mooseData.speed;
            mooseSpeedIncreaseRate = mooseData.speedIncreaseRate;
            mooseSpeedDecreaseRate = mooseData.speedDecreaseRate;
            mooseDetectionRadius = mooseData.detectionRadius;
            mooseDetectionSpeed = mooseData.detectionSpeed;

        }
    }

    private void Update()
    {
        CalmBehavior();
    }
    void CalmBehavior()
    {
        


        if (Vector3.Distance(transform.position, player.transform.position) <= mooseDetectionRadius)
        {
            mooseIsAlerted = true;
            AlertBehavior();
            mooseIsCalm = false;
        }
    }
    void AlertBehavior()
    {
        // À implémenter : logique de comportement d'alerte de l'orignal
    }
    void ChaseBehavior()
    {
        // À implémenter : logique de comportement de poursuite de l'orignal
    }
}

