using UnityEngine;

public class TorchLightManager : MonoBehaviour
{
    // Gestion de la lampe torche du joueur
    [Header("Torch Light Settings")]
   private Light torchLight;  
   public float batteryLife = 100f;
   public float batteryMax = 100f;
   private float batteryDrainRate = 5f; 
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
            batteryLife -= batteryDrainRate * Time.deltaTime;
            batteryLife = Mathf.Clamp(batteryLife, 0f, batteryMax);
            if (batteryLife <= 0f)
            {
                torchLight.enabled = false;
                lightIsOn = false;
            }
        }
    }
}
