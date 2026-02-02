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
       
        if (player != null && player.PlayerSanityLevel < 60 && !mooseIsInstantiate)
        {
            Debug.Log("Instantiate moose");
           
            mooseIsInstantiate = true;
        }
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
