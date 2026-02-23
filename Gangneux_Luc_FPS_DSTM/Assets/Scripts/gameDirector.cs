using System.Collections.Generic;
using UnityEngine;

public class gameDirector : MonoBehaviour
{
    [SerializeField] private Fps_Character player;
    [SerializeField] private GameObject moosePrefab;
    private bool mooseIsInstantiate;

    [Header("Recordings Spawn Settings")]
    public GameObject recordingsPrefab;
    public Transform[] spawnPoints;

    void Update()
    {
        InstantiateRecordings();
    }

    void InstantiateRecordings() // Instancie 5 enregistrements parmi les points de spawn définis lorsque la barre d'espace est pressée
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

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

    void PlayerDeath()
    {
        // Show death screen, pause the game and offer restart option
    }

    private void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            // Show pause menu UI
        }
        else
        {
            Time.timeScale = 1f;
            // Hide pause menu UI
        }
    }
}
