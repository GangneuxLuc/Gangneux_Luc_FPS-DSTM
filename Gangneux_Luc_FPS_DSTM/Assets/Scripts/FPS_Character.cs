using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class Fps_Character : MonoBehaviour
{
    // Script de mouvement du joueur avec gestion de l'endurance et de la santé mentale
    [Header("Input Values")]
    [SerializeField] private float Horizontal;
    [SerializeField] private float Vertical;

    [Header("Player Inventory")]
    [SerializeField] private TorchLightManager torchLightManager;
    [SerializeField] private TMP_Text Recordings; // UI pour afficher le nombre de records trouvés et restants
    private int recordingsFound = 0; // Compteur de records trouvés

    [Header("Player States and stats")]
    [SerializeField] private Player_data playerData;
    [SerializeField] private bool playerIsMoving;
    [SerializeField] private bool playerIsWalking;
    [SerializeField] private bool playerIsSprinting;
    [SerializeField] private float playerWalkSpeed;
    [SerializeField] private float playerSprintSpeed;
    [SerializeField] private float playerStaminaLevel;
    [SerializeField] private float playerStaminaMax;
    [SerializeField] private float playerStaminaIncreaseRate; // récupération
    [SerializeField] private float playerStaminaDecreaseRate; // décrémentation lors du sprint
    [SerializeField] private float playerSanityLevel;
    [SerializeField] private float playerSanityMax;
    [SerializeField] private float playerSanityMin;
    [SerializeField] private float playerSanityDecreaseRate;
    [SerializeField] private float playerSanityIncreaseRate;

    // Exposer en publics en lecture seule pour d'autres scripts
    public float PlayerStaminaLevel => playerStaminaLevel;
    public float PlayerStaminaMax => playerStaminaMax;
    public float PlayerSanityLevel => playerSanityLevel;
    public float PlayerSanityMax => playerSanityMax;
    public bool PlayerIsSprinting => playerIsSprinting;


    private void Awake() // Initialisation des valeurs
    {
        // Récupère les valeurs depuis le ScriptableObject si assigné
        if (playerData != null)
        {
            playerIsWalking = playerData.isWalking;
            playerIsSprinting = playerData.isSprinting;
            playerWalkSpeed = playerData.walkSpeed;
            playerSprintSpeed = playerData.sprintSpeed;
            playerStaminaLevel = playerData.staminaLevel;
            playerStaminaMax = playerData.staminaMax;
            playerStaminaIncreaseRate = playerData.staminaIncreaseRate;
            playerStaminaDecreaseRate = playerData.staminaDecreaseRate;
            playerSanityLevel = playerData.sanityLevel;
            playerSanityMax = playerData.sanityMax;
            playerSanityMin = playerData.sanityMin;
            playerSanityDecreaseRate = playerData.sanityDecreaseRate;
            playerSanityIncreaseRate = playerData.sanityIncreaseRate;
        }

        playerIsMoving = false;
    }


    void Update() // J'appelle mes méthodes à chaque frame
    {
      HandleMovement();
      Sanity();
        playerStaminaMax = playerSanityLevel;
      if (playerStaminaLevel > playerStaminaMax) playerStaminaLevel = playerStaminaMax;

    }

    private void HandleMovement() // Gérer le mouvement du joueur
    {
        if (!gameDirector.inputsEnabled) return;
        // Entrées instantanées (pas de lissage)
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(rawH, rawV);


        // Normalisation pour que la diagonale ne soit pas plus rapide
        if (input.sqrMagnitude > 1f) input.Normalize();

        // Gestion du sprint en fonction de l'input et de la stamina
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);
        bool tryingToSprint = sprintInput && input.sqrMagnitude > 0f && playerStaminaLevel > 0f;
        playerIsSprinting = tryingToSprint;

        // Gestion de la stamina : drain si sprint, récupération sinon
        if (playerIsSprinting)
        {
            playerStaminaLevel -= playerStaminaDecreaseRate * Time.deltaTime;
            if (playerStaminaLevel <= 0f)
            {
                playerStaminaLevel = 0f;
                playerIsSprinting = false; // plus assez d'endurance
            }
        }
        else
        {
            if (playerStaminaLevel < playerStaminaMax)
            {
                playerStaminaLevel += playerStaminaIncreaseRate * Time.deltaTime;
                if (playerStaminaLevel > playerStaminaMax) playerStaminaLevel = playerStaminaMax;

            }
        }

       

        float speed = playerIsSprinting ? playerSprintSpeed : playerWalkSpeed;

       transform.Translate(new Vector3(input.x, 0f, input.y) * speed * Time.deltaTime);
        playerIsMoving = input.sqrMagnitude > 0f;

    }
    void Inventory () // Gérer l'inventaire du joueur (ramassage d'objets, utilisation, etc.)
    {
        // Cette méthode peut être développée pour gérer les interactions avec les objets dans le jeu
    }
    void OnCollisionEnter (Collision collision) // Gérer les collisions avec les ennemis ou les objets qui affectent la santé mentale
    {
        if (collision.gameObject.CompareTag("Batteries"))
        {
            playerSanityLevel += 20f; // Gain de santé mentale en ramassant les piles
            if (playerSanityLevel > playerSanityMax) playerSanityLevel = playerSanityMax;
            torchLightManager.BatteryLife +=50f; // Recharge la batterie de la lampe torche
            Destroy(collision.gameObject); // Détruire l'objet après ramassage
        }
 
       if (collision.gameObject.CompareTag("Recording"))
       {
            recordingsFound++; // Incrémente le nombre de records trouvés
            playerSanityLevel += 60f; // Gain de santé mentale en ramassant les enregistrements
            if (playerSanityLevel > playerSanityMax) playerSanityLevel = playerSanityMax;
            StartCoroutine(RecordingUI());
            Recordings.text = recordingsFound + "/" + "6"; // Met à jour le texte de l'UI  
            Destroy(collision.gameObject); // Détruire l'objet après ramassage    
       }
        
    }

    IEnumerator RecordingUI () // Coroutine pour afficher le nombre de records trouvés pendant un certain temps
    {
        Recordings.enabled = true;
        yield return new WaitForSeconds(3f); // Affiche le texte pendant 3 secondes
        Recordings.enabled = false;
    }

    private bool IsPlayerInDarkArea() // Vérifier si le joueur est dans une zone sombre
    {
        // Cette méthode peut être développée pour détecter les zones sombres et ajuster la santé mentale en conséquence
        return false; // Placeholder, à remplacer par la logique réelle
    }

    private float CalculateSanityChange() // Calculer le changement de santé mentale en fonction des conditions du jeu
    {
        float sanityChange = 0f;
        if (IsPlayerInDarkArea())
        {
            sanityChange -= playerSanityDecreaseRate * Time.deltaTime; // Perte de santé mentale dans les zones sombres
        }
        else
        {
            sanityChange += playerSanityIncreaseRate * Time.deltaTime; // Récupération de santé mentale dans les zones éclairées
        }
        return sanityChange;
    }

    private void StealthAfterChase()
    {

        // Cette méthode peut être développée pour gérer les mécaniques de furtivité après une poursuite avec un ennemi
    }

    private void Sanity() // Gérer la santé mentale du joueur
    {
        playerSanityLevel -= playerSanityDecreaseRate * Time.deltaTime;
        if (playerSanityLevel < playerSanityMin) playerSanityLevel = playerSanityMin;

    }
}


