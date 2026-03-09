using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class gameDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject moosePrefab;
    [SerializeField] private MenuManager menuManager;

    public AudioSource mooseAudioSource;
    public AudioClip mooseScreamerClip;
    bool moosePlaced = false;
    bool isGameWon = false;


    [Header("Recordings Spawn Settings")]
    public GameObject recordingsPrefab;
    public Transform[] spawnPoints;

    [Header("Player Infos")]
    public bool isPlayerDead;

    private void Awake()
    {
       GetComponent<MenuManager>();

    }

    private void Start()
    {
        
        Debug.Log("GameDirector Start: Initializing game state...");
        Time.timeScale = 1f; // Assure que le temps est normal au démarrage
        isPlayerDead = false; // Assure que le joueur n'est pas considéré comme mort au démarrage
        InstantiateRecordings();
        Debug.Log("Recordings instantiated at random spawn points.");
        if (moosePlaced == false) PlaceMoose();
        else return;


    }
    private void PlaceMoose() // Génère un point de spawn aléatoire pour le moose dans une zone comprise dans le terrain
    {
        if (!isPlayerDead)
        {
            if (moosePlaced == false)
            {
                moosePlaced = true;
                // Si le moose a déjà été placé, on ne le repositionne pas
                int randomSpawnPointx = Random.Range(40, 300);
                int randomSpawnPointz = Random.Range(40, 300);
                Debug.Log($"Random spawn point for moose: ({randomSpawnPointx}, {randomSpawnPointz})");
                moosePrefab.transform.position = new Vector3(randomSpawnPointx, 0f, randomSpawnPointz);
            }
        }

    }


    void Update()
    {
        HandlePauseInput();
        GameOver();
        EndGame();
    }
    void EndGame()
    {
        if (player.GetComponent<Fps_Character>().recordingsFound >= 6)
        {
            if (isGameWon) return; // Empêche de relancer la séquence de fin si elle a déjà été déclenchée
            GameObject endMenu =Instantiate(menuManager.EndMenu);
            var canvas = GameObject.Find("UI");
            if (canvas != null)
            {
                endMenu.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Time.timeScale = 0f; // Freeze le jeu
            isGameWon = true;
        }
       
    }
    void InstantiateRecordings() // Instancie 5 enregistrements parmi les points de spawn définis 
    {
        int spawnCount = Mathf.Min(5, spawnPoints.Length);
        var usedIndices = new HashSet<int>();
        int attempts = 0;
        int maxAttempts = spawnPoints.Length * 3;

        for (int spawned = 0; spawned < spawnCount && attempts < maxAttempts; attempts++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            if (usedIndices.Contains(randomIndex))
                continue;

            Vector3 spawnPos = spawnPoints[randomIndex].position;

            // Vérifie s'il y a déjà un enregistrement proche (évite d'instancier deux fois au même endroit)
            float checkRadius = 0.5f;
            bool occupied = false;
            Collider[] hits = Physics.OverlapSphere(spawnPos, checkRadius);
            foreach (var hit in hits)
            {
                if (hit.gameObject.CompareTag(recordingsPrefab.tag) || hit.gameObject.name.Contains(recordingsPrefab.name))
                {
                    occupied = true;
                    break;
                }
            }

            if (occupied)
                continue;

            // Instancie à la position choisie
            Instantiate(recordingsPrefab, spawnPos, Quaternion.identity);
            usedIndices.Add(randomIndex);
            spawned++;
        }
    }

    private void HandlePauseInput()
    {
        // Empêche d'ouvrir le menu de pause si l'écran Game Over est actif
        if (isPlayerDead) return;

        if (Input.GetKeyDown(KeyCode.Escape) && menuManager.isPaused == false)
        {
            menuManager.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menuManager.isPaused == true)
        {
            menuManager.ResumeGame();
        }
    }




    public void GameOver()
    {
        if (isPlayerDead)
        {
            Debug.Log("Player is dead. Starting Game Over sequence...");
            StartCoroutine(GameOverCoroutine());
            isPlayerDead = false; // Empêche de relancer la coroutine à chaque frame après la mort du joueur
        }

        else if (player.GetComponent<Fps_Character>().PlayerSanityLevel <= 0)
        {
            StartCoroutine(GameOverCoroutine());
        }
    }

    IEnumerator GameOverCoroutine()
    {
        moosePrefab = GameObject.FindWithTag("Moose"); // Assure que la référence est à jour au moment du Game Over
        StopMooseBehavior();

        Debug.Log("Game Over triggered in gameDirector.");

        mooseAudioSource.PlayOneShot(mooseScreamerClip); // Joue le son de screamer
        GameObject screamer = Instantiate(menuManager.Screamer); // Instancie le prefab
                                                                 // Cherche "UI" 
        var canvas = GameObject.Find("UI");
        if (canvas != null)
        {
            screamer.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
        }
        yield return new WaitForSeconds(3f);
        mooseAudioSource.Stop(); // Arrête le son du moose après le screamer



        GameObject gameOver = Instantiate(menuManager.gameOverUIPrefab);
        if (canvas != null)
        {
            gameOver.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
        }
        // Rend le curseur visible pour permettre le clic sur les boutons UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // Freeze le jeu
    }
    void StopMooseBehavior()
    {
        if (moosePrefab != null)
        {
            var mooseAI = moosePrefab.GetComponent<StateManager>();
            if (mooseAI != null)
            {
                mooseAI.enabled = false; // Désactive le script de comportement du moose
            }
        }
    }
}
