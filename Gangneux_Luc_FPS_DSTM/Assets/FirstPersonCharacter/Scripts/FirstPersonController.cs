    using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    //[RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;

        private Camera m_Camera;
        private bool m_Jump;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_AudioSource = GetComponent<AudioSource>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        private void Update()
        {
            RotateView();
            if (!m_Jump) m_Jump = Input.GetButtonDown("Jump");

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlaySound(m_LandSound, 0.5f);
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            GetInput(out float speed);
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out RaycastHit hitInfo,
                m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlaySound(m_JumpSound);
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            ProgressStepCycle(speed);
            m_MouseLook.UpdateCursorLock();
        }

        private void PlaySound(AudioClip clip, float nextStepOffset = 0f)
        {
            if (clip == null) return;
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
            if (nextStepOffset > 0f) m_NextStep = m_StepCycle + nextStepOffset;
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                               Time.fixedDeltaTime;
            }

            if (m_StepCycle <= m_NextStep) return;

            m_NextStep = m_StepCycle + m_StepInterval;
            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded || m_FootstepSounds == null || m_FootstepSounds.Length == 0) return;
            if (m_FootstepSounds.Length == 1)
            {
                m_AudioSource.PlayOneShot(m_FootstepSounds[0]);
                return;
            }

            int n = Random.Range(1, m_FootstepSounds.Length);
            AudioClip clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(clip);
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = clip;
        }

        private void GetInput(out float speed)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

#if !MOBILE_INPUT
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
            if (m_Input.sqrMagnitude > 1) m_Input.Normalize();
        }

        private void RotateView() => m_MouseLook.LookRotation(transform, m_Camera.transform);

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (m_CollisionFlags == CollisionFlags.Below) return;
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
