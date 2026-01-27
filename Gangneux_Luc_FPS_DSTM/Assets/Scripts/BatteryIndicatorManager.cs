using UnityEngine;
using UnityEngine.UI;

// A finir + rajouter une image dans l'UI
public class BatteryIndicatorManager : MonoBehaviour
{
   [SerializeField] private Image BatteryIndicator;
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
        
    }

    
}
