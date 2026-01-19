using UnityEngine;
using FPS_Controller;

[CreateAssetMenu(fileName = "Player_data", menuName = "Scriptable Objects/Player_data")]
public class Player_data : ScriptableObject
{
    public bool isWalking;
    public float walkSpeed;
    public float runSpeed;
    [Range(0f, 1f)] public float runstepLenghten;
    public float stickToGroundForce;
    public float gravityMultiplier;
    public MouseLook mouseLook;
    public float stepInterval;
    public AudioClip[] footstepSounds;
    public AudioClip landSound;

    // Stamina fields (ajoutés pour compatibilité)
    public float staminaLevel;
    public float staminaMax;
    public float staminaIncreaseRate; // récupération
    public float staminaDecreaseRate; // décrémentation lors du sprint

    // Sanity
    public float sanityLevel;
    public float sanityDecreaseRate;
    public float sanityIncreaseRate;
}
