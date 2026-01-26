using UnityEngine;
namespace Character
{
    public class FPS_Camera : MonoBehaviour
    {
        public Transform playerBody;

        [Header("Input Settings")]
        [SerializeField] private float mouseX;
        [SerializeField] private float mouseY;


       

        [SerializeField] private Player_data playerData;
        [Header("Camera Settings")]
        [SerializeField] private float playerSensitivityX;
        [SerializeField] private float playerSensitivityY;
        [SerializeField] private float playerMinRotationY;
        [SerializeField] private float playerMaxRotationY;
        [SerializeField] private float playerDeadzone;

        private float rotationX;
        private float rotationY;

        private void Awake()
        {
            // Récupère les valeurs depuis le ScriptableObject si assigné
            if (playerData != null)
            {
                playerSensitivityX = playerData.sensitivityX;
                playerSensitivityY = playerData.sensitivityY;
                playerMinRotationY = playerData.minRotationY;
                playerMaxRotationY = playerData.maxRotationY;
                playerDeadzone = playerData.deadzone;
            }
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
            // Applique la rotation horizontale au corps du joueur
            if (playerBody != null)
            {
                transform.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX * playerSensitivityX);
            }

        }
    }
      
}