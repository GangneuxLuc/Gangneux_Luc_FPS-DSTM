using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameDirector : MonoBehaviour
{
    [Header("Scene and UI Settings")]
    public string sceneName;
    public GameObject gameOverUIPrefab;
    public GameObject PauseMenu;

    [Header("References")]
    [SerializeField] private Fps_Character player;
    [SerializeField] private GameObject moosePrefab;
    

    // contrôle global des entrées
    public static bool inputsEnabled = true;
    private bool isPaused = false;
    GameObject currentPauseMenu;


    [Header("Recordings Spawn Settings")]
    public GameObject recordingsPrefab;
    public Transform[] spawnPoints;

    [Header("Player Infos")]
    public bool isPlayerDead;


    private void Start()
    {
        InstantiateRecordings();
    }

    private void Awake()
    {
        
    }

    void Update()
    { 
        HandlePauseInput();     
        GameOver();
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

        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
        {
           PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true)
        {
           ResumeGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        inputsEnabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PauseMenu == null) return;

        // Instancier une seule fois
        if (currentPauseMenu == null)
        {
            currentPauseMenu = Instantiate(PauseMenu);
            var canvas = GameObject.Find("UI") ?? GameObject.Find("Canvas");
            if (canvas != null)
            {
                currentPauseMenu.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
            }

        }
    }

    public void ResumeGame()
    {
        currentPauseMenu = GameObject.FindWithTag("PauseMenu");
        isPaused = false;
        Time.timeScale = 1f;
        inputsEnabled = true;
        Destroy(currentPauseMenu);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentPauseMenu = null;
    }
     
    
    public void GameOver()
    {
        if (isPlayerDead)
        {
            isPlayerDead = false; // Reset the flag to prevent multiple triggers
            Debug.Log("Game Over triggered in gameDirector.");
            Time.timeScale = 0f; // Freeze the game
            inputsEnabled = false; // Désactive les entrées globalement

            // Rendre le curseur visible pour permettre le clic sur les boutons UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameObject ui = Instantiate(gameOverUIPrefab); // Instancie le prefab
            // Cherche "UI" puis "Canvas" comme fallback
            var canvas = GameObject.Find("UI") ?? GameObject.Find("Canvas");
            if (canvas != null)
            {
                ui.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
            }
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Reprend le temps
        inputsEnabled = true; // Réactive les entrées globalement
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recharge la scène actuelle
    }
    public void TitleStartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
