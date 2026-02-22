using UnityEngine;

public class TorchLightManager : MonoBehaviour
{

    // Gestion de la lampe torche du joueur
    [Header("Torch Light Settings")]
    public float BatteryLife = 100f;
    public float BatteryMax = 100f;
    private float BatteryDrainRate = 2.5f;
    private Light torchLight;  
    private bool lightIsOn = false;

  
    void Start() // Initialisation de la lampe torche
    {
        torchLight = GetComponent<Light>();
    }
    void Update() // J'appelle mes méthodes à chaque frame
    {
      Light();
      Battery();
    }
    private void Light() // Allumer/éteindre la lampe torche avec la touche F
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            torchLight.enabled = !torchLight.enabled;
            lightIsOn = torchLight.enabled;
        }
    }


    private void Battery() // Gérer la batterie de la lampe torche
    {
        if (lightIsOn)
        {
            BatteryLife -= BatteryDrainRate * Time.deltaTime;
            BatteryLife = Mathf.Clamp(BatteryLife, 0f, BatteryMax);
            if (BatteryLife <= 0f)
            {
                torchLight.enabled = false;
                lightIsOn = false;
            }
        }
    }
}
