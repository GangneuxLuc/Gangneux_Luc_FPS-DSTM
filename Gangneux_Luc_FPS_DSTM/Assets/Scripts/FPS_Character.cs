using UnityEngine;
namespace Character
{
    public class FPS_Character : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float mouseX;
        [SerializeField] private float mouseY;
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
        [SerializeField] private float playerSanityDecreaseRate;
        [SerializeField] private float playerSanityIncreaseRate;

        // Exposer des accesseurs publics en lecture seule pour d'autres scripts
        public float PlayerStaminaLevel => playerStaminaLevel;
        public float PlayerStaminaMax => playerStaminaMax;
        public bool PlayerIsSprinting => playerIsSprinting;

        [Header("Camera Settings")]
        [SerializeField] private float playerSensitivityX = 5f;
        [SerializeField] private float playerSensitivityY = 3f;
        [SerializeField] private float playerMinRotationY = -30f;
        [SerializeField] private float playerMaxRotationY = 15f;
        [SerializeField] private float playerDeadzone = 0.15f;

        private float rotationX;
        private float rotationY;

        private void Awake()
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
                playerSanityDecreaseRate = playerData.sanityDecreaseRate;
                playerSanityIncreaseRate = playerData.sanityIncreaseRate;
            }

            playerIsMoving = false;
        }

        void Start()
        {
            // Verrouille le curseur pour un contrôle FPS classique
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            rotationX += mouseX * playerSensitivityX;
            rotationY -= mouseY * playerSensitivityY;
            rotationY = Mathf.Clamp(rotationY, playerMinRotationY, playerMaxRotationY);

            // Applique la rotation directement sur la caméra
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }

        private void HandleMovement()
        {
            // Entrées instantanées (pas de lissage)
            float rawH = Input.GetAxisRaw("Horizontal");
            float rawV = Input.GetAxisRaw("Vertical");

            Vector2 input = new Vector2(rawH, rawV);

            // Deadzone pour éviter les petites valeurs résiduelles
            if (input.magnitude < playerDeadzone) input = Vector2.zero;

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

            // Déplace selon la direction de la caméra, en ignorant la composante verticale
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 move = (right * input.x + forward * input.y) * speed * Time.deltaTime;
            transform.position += move;

            // Mise à jour des champs sérialisés pour l'inspector
            Horizontal = input.x;
            Vertical = input.y;
            playerIsMoving = input.sqrMagnitude > 0f;
        }
    }
}