using Character;
using UnityEngine;
using UnityEngine.UI;


public class StaminaBarManager : MonoBehaviour
{
    // Gestion de la barre d'endurance du joueur sur l'UI
    [SerializeField] private Fps_Character player;
    [SerializeField] private Image staminaBarImage;
    [SerializeField] private Image staminaBarBackgroundImage;

   

    private void Start() // Récupération du script Fps_Character
    {
        if (player != null) player.GetComponent<Fps_Character>();
    }

    void Update() // Mise à jour de la barre d'endurance
    {
        if (player == null) return;

        float stamina = player.PlayerStaminaLevel;
        float maxStamina = player.PlayerStaminaMax;
        if (maxStamina <= 0f) maxStamina = 1f;

        if (staminaBarImage != null)
            staminaBarImage.fillAmount = Mathf.Clamp01(stamina / maxStamina);

        bool show = stamina < maxStamina; // Afficher uniquement si l'endurance n'est pas au maximum
        if (staminaBarImage != null) staminaBarImage.enabled = show;
        if (staminaBarBackgroundImage != null) staminaBarBackgroundImage.enabled = show;
    } 
}
