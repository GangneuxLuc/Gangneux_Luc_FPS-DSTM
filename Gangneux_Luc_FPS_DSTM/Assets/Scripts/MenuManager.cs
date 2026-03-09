using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // contrôle global des entrées
    public static bool inputsEnabled = true;
    public bool isPaused = false;
    GameObject currentPauseMenu;

    [Header("Scene and UI Settings")]
    public string sceneName;
    public GameObject Screamer;
    public GameObject EndMenu;
    public GameObject gameOverUIPrefab;
    public GameObject PauseMenu;
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

    public void PauseGame()
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
            var canvas = GameObject.Find("UI");
            if (canvas != null)
            {
                currentPauseMenu.transform.SetParent(canvas.transform, false); // Place le UI sous le Canvas
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

