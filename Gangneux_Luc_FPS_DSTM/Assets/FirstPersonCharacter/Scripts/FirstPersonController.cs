using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS_Controller
{

    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] Player_data playerData;


        [Header("Movement Settings")]

        [SerializeField] private bool playerIsWalking;
        [SerializeField] private float playerWalkSpeed;
        [SerializeField] private float playerRunSpeed;
        [SerializeField] [Range(0f, 1f)] private float playerRunstepLenghten;
        [SerializeField] private float playerStickToGroundForce;
        [SerializeField] private float playerGravityMultiplier;
        [SerializeField] private MouseLook playerMouseLook;
        [SerializeField] private float playerStepInterval;
        [SerializeField] private AudioClip[] playerFootstepSounds;
        [SerializeField] private AudioClip playerLandSound;

        private Camera playerCameraComponent;
        private Vector2 playerInputVector;
        private Vector3 playerMoveDir = Vector3.zero;
        private CharacterController playerCharacterController;
        private CollisionFlags playerCollisionFlags;
        private bool playerPreviouslyGrounded;
        private Vector3 playerOriginalCameraPosition;
        private float playerStepCycle;
        private float playerNextStep;
        private AudioSource playerAudioSource;

        private void Start()
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
            playerLandSound = playerData.landSound;





            playerCharacterController = GetComponent<CharacterController>();
            playerAudioSource = GetComponent<AudioSource>();
            playerCameraComponent = Camera.main;
            playerOriginalCameraPosition = playerCameraComponent.transform.localPosition;
            playerStepCycle = 0f;
            playerNextStep = playerStepCycle / 2f;
            playerMouseLook.Init(transform, playerCameraComponent.transform);
        }

        private void Update()
        {
            RotateView();

            if (!playerPreviouslyGrounded && playerCharacterController.isGrounded)
            {
                PlaySound(playerLandSound, 0.5f);
                playerMoveDir.y = 0f;
            }

            playerPreviouslyGrounded = playerCharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            GetInput(out float speed);
            Vector3 desiredMove = transform.forward * playerInputVector.y + transform.right * playerInputVector.x;

            Physics.SphereCast(transform.position, playerCharacterController.radius, Vector3.down, out RaycastHit hitInfo,
                playerCharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            playerMoveDir.x = desiredMove.x * speed;
            playerMoveDir.z = desiredMove.z * speed;

            if (playerCharacterController.isGrounded)
            {
                playerMoveDir.y = -playerStickToGroundForce;
            }
            else
            {
                playerMoveDir += Physics.gravity * playerGravityMultiplier * Time.fixedDeltaTime;
            }

            playerCollisionFlags = playerCharacterController.Move(playerMoveDir * Time.fixedDeltaTime);
            ProgressStepCycle(speed);
            playerMouseLook.UpdateCursorLock();
        }

        private void PlaySound(AudioClip clip, float nextStepOffset = 0f)
        {
            if (clip == null) return;
            playerAudioSource.clip = clip;
            playerAudioSource.Play();
            if (nextStepOffset > 0f) playerNextStep = playerStepCycle + nextStepOffset;
        }

        private void ProgressStepCycle(float speed)
        {
            if (playerCharacterController.velocity.sqrMagnitude > 0 && (playerInputVector.x != 0 || playerInputVector.y != 0))
            {
                playerStepCycle += (playerCharacterController.velocity.magnitude + (speed * (playerIsWalking ? 1f : playerRunstepLenghten))) *
                               Time.fixedDeltaTime;
            }

            if (playerStepCycle <= playerNextStep) return;

            playerNextStep = playerStepCycle + playerStepInterval;
            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!playerCharacterController.isGrounded || playerFootstepSounds == null || playerFootstepSounds.Length == 0) return;
            if (playerFootstepSounds.Length == 1)
            {
                playerAudioSource.PlayOneShot(playerFootstepSounds[0]);
                return;
            }

            int n = Random.Range(1, playerFootstepSounds.Length);
            AudioClip clip = playerFootstepSounds[n];
            playerAudioSource.PlayOneShot(clip);
            playerFootstepSounds[n] = playerFootstepSounds[0];
            playerFootstepSounds[0] = clip;
        }

        private void GetInput(out float speed)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

    #if !MOBILE_INPUT
            playerIsWalking = !Input.GetKey(KeyCode.LeftShift);
    #endif
            speed = playerIsWalking ? playerWalkSpeed : playerRunSpeed;
            playerInputVector = new Vector2(horizontal, vertical);
            if (playerInputVector.sqrMagnitude > 1) playerInputVector.Normalize();
        }

        private void RotateView() => playerMouseLook.LookRotation(transform, playerCameraComponent.transform);

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (playerCollisionFlags == CollisionFlags.Below) return;
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;
            body.AddForceAtPosition(playerCharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
