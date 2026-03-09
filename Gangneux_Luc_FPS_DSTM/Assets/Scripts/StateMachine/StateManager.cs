using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] gameDirector gameDirector;
    public State currentState;

    [Header("SFX")]
    public AudioSource mooseAudioSource;
    public AudioClip[] mooseSFXClips;

    [Header("Audio Controls")]
    [Range(0f, 1f)] public float MooseClip = 1f; // volume global (inspector)


    [Header("Cooldown after Chase -> Wander")]
    [Tooltip("Temps (s) pendant lequel le moose ne peut pas devenir Alert après être passé de Chase à Wander.")]
    [SerializeField] private float alertCooldownAfterChase = 5f;
    private float lastChaseExitTime = -Mathf.Infinity;

    private Coroutine sfxCoroutine;

    private void Awake()
    {

    }
    private void Start()
    {
        // Démarre la coroutine une seule fois
        if (sfxCoroutine == null)
            sfxCoroutine = StartCoroutine(SFX());
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
        }
    }

    private IEnumerator SFX() // Coroutine pour jouer des sons aléatoires du moose à intervalles aléatoires
    {
        Debug.Log("SFX Coroutine started.");
        if (mooseAudioSource == null || mooseSFXClips == null || mooseSFXClips.Length == 0)
        yield break;
        mooseAudioSource.volume = MooseClip; // applique le volume global défini dans l'inspector
        while (true)
        {
            int randomSFX = Random.Range(0, mooseSFXClips.Length);
            AudioClip clip = mooseSFXClips[randomSFX];
            if (clip != null)
                mooseAudioSource.PlayOneShot(clip);

            float waitSeconds = Random.Range(30f, 120f);
            yield return new WaitForSecondsRealtime(waitSeconds);

            
        }
    }
}

