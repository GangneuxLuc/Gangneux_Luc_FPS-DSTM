using UnityEngine;

[CreateAssetMenu(fileName = "Moose_Data", menuName = "Scriptable Objects/Moose_Data")]
public class Moose_Data : ScriptableObject
{
    [Header("Moose States and stats")]
    public float speed;
    public float speedIncreaseRate;
    public float speedDecreaseRate;
    public float detectionRadius;
    public float detectionSpeed;
}
