using UnityEngine;

public class Fps_Character : MonoBehaviour
{
    // Script de mouvement du joueur avec gestion de l'endurance et de la santé mentale
    [Header("Input Values")]
    [SerializeField] private float Horizontal;
    [SerializeField] private float Vertical;

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

    private void Sanity() // Gérer la santé mentale du joueur
    {
        playerSanityLevel -= playerSanityDecreaseRate * Time.deltaTime;
    }
}


