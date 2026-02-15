using UnityEngine;
using UnityEngine.UI;

// A finir + rajouter une image dans l'UI
public class BatteryIndicatorManager : MonoBehaviour
{
   [SerializeField] private Image batteryImage;
   [SerializeField] private TorchLightManager torchLightManager;
    void Start()
    {
        if (torchLightManager == null)
        {
            torchLightManager = GetComponent<TorchLightManager>();
        }
    }
    void Update()
    {
        float batteryLevel = torchLightManager.batteryLife;
        float batteryMax = torchLightManager.batteryMax;
        batteryImage.fillAmount = Mathf.Clamp01(batteryLevel / batteryMax);
    }

    
}
