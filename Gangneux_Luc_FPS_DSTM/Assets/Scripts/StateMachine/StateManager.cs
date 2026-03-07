using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] gameDirector gameDirector;
    public State currentState;

    [Header("Cooldown after Chase -> Wander")]
    [Tooltip("Temps (s) pendant lequel le moose ne peut pas devenir Alert après être passé de Chase à Wander.")]
    [SerializeField] private float alertCooldownAfterChase = 5f;
    private float lastChaseExitTime = -Mathf.Infinity;

    private void Awake()
    {
        
    }

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();
        // ne switcher que si nextState est différent pour éviter re-enabled inutile
        if (nextState != null && nextState != currentState)
        {
            // Bloquer la transition vers AlertState si le cooldown après Chase est actif
            if (nextState is AlertState && Time.time - lastChaseExitTime < alertCooldownAfterChase)
            {
                Debug.Log($"Transition vers AlertState bloquée pendant cooldown ({alertCooldownAfterChase}s).");
                return;
            }

            SwitchToNextState(nextState);
        }
    }

    private void SwitchToNextState(State nextState)
    {
        // Si on quitte ChaseState pour aller vers WanderState, enregistre le timestamp pour le cooldown
        if (currentState is ChaseState && nextState is WanderState)
        {
            lastChaseExitTime = Time.time;
        }

        // désactive l'ancien état s'il existe
        if (currentState != null)
            currentState.enabled = false;

        currentState = nextState;

        // active le nouvel état s'il existe
        if (currentState != null && !currentState.enabled)
            currentState.enabled = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has been caught by the moose! Game Over.");
            gameDirector.isPlayerDead = true;
            gameDirector.GameOver();

        }
    }
    
}

