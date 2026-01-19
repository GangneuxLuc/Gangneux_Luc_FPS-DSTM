using UnityEngine;
using UnityEngine.UI;


public class StaminaBarManager : MonoBehaviour
{
    [SerializeField] GameObject playerController;

    float stamina = 10;
    float maxStamina = 100;
    float staminaRegenRate;
    public Image staminaBarImage;
    public Image staminaBarBackgroundImage;
    private bool isSprinting;

    private void Start()
    {
     
    }


    // Update is called once per frame
    void Update()
    {
        

        stamina = playerController.GetComponent<FPS_Controller.FirstPersonController>().playerStaminaLevel;
        maxStamina = playerController.GetComponent<FPS_Controller.FirstPersonController>().playerStaminaMax;
        staminaBarImage.fillAmount = stamina / maxStamina;

    

        if (stamina < maxStamina)
        { staminaBarImage.enabled = true; staminaBarBackgroundImage.enabled = true; }
        else
        { staminaBarImage.enabled = false; staminaBarBackgroundImage.enabled = false; }
    }
}
