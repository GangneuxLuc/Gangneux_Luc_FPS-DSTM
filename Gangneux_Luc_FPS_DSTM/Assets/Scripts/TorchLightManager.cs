using UnityEngine;

public class TorchLightManager : MonoBehaviour
{
   private Light torchLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            torchLight.enabled = !torchLight.enabled;
        }
    }
}
