using UnityEngine;

public class gameDirector : MonoBehaviour
{
    [SerializeField] private Fps_Character player;
    [SerializeField] private GameObject moosePrefab;

    [Header("Spawn distance (meters)")]
    [SerializeField] private float spawnDistanceMin = 8f;
    [SerializeField] private float spawnDistanceMax = 12f;

    private bool mooseIsInstantiate;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InstantiateMoose();
        }
        if (player != null && player.PlayerSanityLevel < 60 && !mooseIsInstantiate)
        {
            Debug.Log("Instantiate moose");
            InstantiateMoose();
            mooseIsInstantiate = true;
        }
    }

    void InstantiateMoose()
    {
        if (moosePrefab == null || player == null) return;

        // Choisir une distance aléatoire dans l'intervalle et un angle aléatoire
        float distance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;

        // Position de départ (au niveau du joueur)
        Vector3 spawnPos = player.transform.position + offset;


 

        Instantiate(moosePrefab, spawnPos , Quaternion.identity);
        mooseIsInstantiate = true;
    }

    void PlayerDeath()
    {
        // Show death screen, pause the game and offer restart option
    }

    void OnDrawGizmos()
    {
        // Visualiser la zone de spawn autour du joueur dans l'éditeur
        Gizmos.color = Color.red;
        Vector3 centre = player != null ? player.transform.position : transform.position;

        // Cercle extérieur et intérieur (min/max)
        Gizmos.DrawWireSphere(centre, spawnDistanceMax);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centre, spawnDistanceMin);
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
