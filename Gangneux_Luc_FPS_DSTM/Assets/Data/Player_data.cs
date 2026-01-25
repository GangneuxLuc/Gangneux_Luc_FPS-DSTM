using UnityEngine;
using Character;


[CreateAssetMenu(fileName = "Player_data", menuName = "Scriptable Objects/Player_data")]
public class Player_data : ScriptableObject
{
    [Header("Player States and stats")]
    public bool isWalking;
    public bool isSprinting;
    public float walkSpeed;
    public float sprintSpeed;
    public float staminaLevel;
    public float staminaMax;
    public float staminaIncreaseRate; // récupération
    public float staminaDecreaseRate; // décrémentation lors du sprint
    public float sanityLevel;
    public float sanityDecreaseRate;
    public float sanityIncreaseRate;


    [Header("Camera Settings")]
    public float sensitivityX;
    public float sensitivityY;
    public float minRotationY;
    public float maxRotationY;
    public float deadzone;


}