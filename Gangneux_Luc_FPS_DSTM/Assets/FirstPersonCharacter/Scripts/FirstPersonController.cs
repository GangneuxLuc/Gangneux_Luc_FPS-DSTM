using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS_Controller
{
    public class FirstPersonController : MonoBehaviour
    {
        public Player_data playerData;

        [Header("Player Data")]
        [SerializeField] private bool playerIsWalking;
        [SerializeField] private float playerWalkSpeed;
        [SerializeField] private float playerRunSpeed;
        [SerializeField] [Range(0f, 1f)] private float playerRunstepLenghten;
        [SerializeField] private float playerJumpSpeed;
        [SerializeField] private float playerStickToGroundForce;
        [SerializeField] private float playerGravityMultiplier;
        [SerializeField] private MouseLook playerMouseLook;
        [SerializeField] private float playerStepInterval;
        [SerializeField] private AudioClip[] playerFootstepSounds;
        [SerializeField] private AudioClip playerJumpSound;
        [SerializeField] private AudioClip playerLandSound;
        public float playerStaminaLevel;
        public float playerstaminaIncreaseRate;
        public float playerstaminaDecreaseRate;
        public float playerStaminaMax;
        public float playerSanityLevel;
        public float playerSanityDecreaseRate;
        public float playerSanityIncreaseRate;

        // Internal runtime fields (kept simple)
        private Camera cameraComponent;
        private bool jump;
        private Vector2 inputVector;
        private Vector3 moveDir = Vector3.zero;
        private CharacterController characterController;
        private CollisionFlags collisionFlags;
        private bool previouslyGrounded;
        private Vector3 originalCameraPosition;
        private float stepCycle;
        private float nextStep;
        private bool jumping;
        private AudioSource audioSource;

        private void Start()
        {
            // Load defaults from playerData if present
            if (playerData != null)
            {
                playerWalkSpeed = playerData.walkSpeed;
                playerRunSpeed = playerData.runSpeed;
                playerIsWalking = playerData.isWalking;
                playerRunstepLenghten = playerData.runstepLenghten;
                playerStickToGroundForce = playerData.stickToGroundForce;
                playerGravityMultiplier = playerData.gravityMultiplier;
                playerMouseLook = playerData.mouseLook;
                playerStepInterval = playerData.stepInterval;
                playerFootstepSounds = playerData.footstepSounds;

                playerStaminaLevel = playerData.staminaLevel;
                playerstaminaIncreaseRate = playerData.staminaIncreaseRate;
                playerstaminaDecreaseRate = playerData.staminaDecreaseRate;
                playerStaminaMax = playerData.staminaMax;

                playerSanityLevel = playerData.sanityLevel;
                playerSanityDecreaseRate = playerData.sanityDecreaseRate;
                playerSanityIncreaseRate = playerData.sanityIncreaseRate;
            }

            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            cameraComponent = Camera.main;
            originalCameraPosition = cameraComponent != null ? cameraComponent.transform.localPosition : Vector3.zero;
            stepCycle = 0f;
            nextStep = stepCycle / 2f;
            jumping = false;

            if (playerMouseLook != null && cameraComponent != null)
                playerMouseLook.Init(transform, cameraComponent.transform);
        }

        private void Update()
        {
            RotateView();

            // Jump input kept (si vous voulez retirer le saut, supprimez ces lignes)
            if (!jump) jump = Input.GetButtonDown("Jump");

            if (!previouslyGrounded && characterController.isGrounded)
            {
                PlaySound(playerLandSound, 0.5f);
                moveDir.y = 0f;
                jumping = false;
            }
            if (!characterController.isGrounded && !jumping && previouslyGrounded)
            {
                moveDir.y = 0f;
            }
            previouslyGrounded = characterController.isGrounded;
        }

        private void FixedUpdate()
        {
            GetInput(out float speed);

            // Manage stamina: decrease when running + moving, recover otherwise
            bool tryingToRun = !playerIsWalking && inputVector.sqrMagnitude > 0f;
            if (tryingToRun && playerStaminaLevel > 0f)
            {
                playerStaminaLevel -= playerstaminaDecreaseRate * Time.fixedDeltaTime;
                if (playerStaminaLevel <= 0f)
                {
                    playerStaminaLevel = 0f;
                    // block running when stamina empty
                    playerIsWalking = true;
                    speed = playerWalkSpeed;
                }
            }
            else
            {
                // recover stamina up to max
                playerStaminaLevel = Mathf.Min(playerStaminaMax, playerStaminaLevel + playerstaminaIncreaseRate * Time.fixedDeltaTime);
            }

            Vector3 desiredMove = transform.forward * inputVector.y + transform.right * inputVector.x;

            if (characterController != null)
            {
                Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out RaycastHit hitInfo,
                    characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                moveDir.x = desiredMove.x * speed;
                moveDir.z = desiredMove.z * speed;

                if (characterController.isGrounded)
                {
                    moveDir.y = -playerStickToGroundForce;
                    if (jump)
                    {
                        moveDir.y = playerJumpSpeed;
                        PlaySound(playerJumpSound);
                        jump = false;
                        jumping = true;
                    }
                }
                else
                {
                    moveDir += Physics.gravity * playerGravityMultiplier * Time.fixedDeltaTime;
                }

                collisionFlags = characterController.Move(moveDir * Time.fixedDeltaTime);
            }

            ProgressStepCycle(speed);

            if (playerMouseLook != null)
                playerMouseLook.UpdateCursorLock();
        }

        private void PlaySound(AudioClip clip, float nextStepOffset = 0f)
        {
            if (clip == null || audioSource == null) return;
            audioSource.clip = clip;
            audioSource.Play();
            if (nextStepOffset > 0f) nextStep = stepCycle + nextStepOffset;
        }

        private void ProgressStepCycle(float speed)
        {
            if (characterController != null && characterController.velocity.sqrMagnitude > 0f && (inputVector.x != 0f || inputVector.y != 0f))
            {
                stepCycle += (characterController.velocity.magnitude + (speed * (playerIsWalking ? 1f : playerRunstepLenghten))) * Time.fixedDeltaTime;
            }

            if (stepCycle <= nextStep) return;

            nextStep = stepCycle + playerStepInterval;
            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (characterController == null || !characterController.isGrounded || playerFootstepSounds == null || playerFootstepSounds.Length == 0) return;
            if (playerFootstepSounds.Length == 1)
            {
                audioSource.PlayOneShot(playerFootstepSounds[0]);
                return;
            }

            int n = Random.Range(1, playerFootstepSounds.Length);
            AudioClip clip = playerFootstepSounds[n];
            audioSource.PlayOneShot(clip);
            playerFootstepSounds[n] = playerFootstepSounds[0];
            playerFootstepSounds[0] = clip;
        }

        private void GetInput(out float speed)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

#if !MOBILE_INPUT
            // If stamina is zero, force walking
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
            playerIsWalking = (playerStaminaLevel <= 0f) ? true : !shiftPressed;
#endif
            speed = playerIsWalking ? playerWalkSpeed : playerRunSpeed;
            inputVector = new Vector2(horizontal, vertical);
            if (inputVector.sqrMagnitude > 1f) inputVector.Normalize();
        }

        private void RotateView()
        {
            if (playerMouseLook != null && cameraComponent != null)
                playerMouseLook.LookRotation(transform, cameraComponent.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (collisionFlags == CollisionFlags.Below) return;
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;
            body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
