using Character;
using UnityEngine;
using UnityEngine.UI;


public class SanityBarManager : MonoBehaviour
{
    [SerializeField] private Fps_Character player;
    [SerializeField] private Image sanityBarImage;
    [SerializeField] private Image sanityBarBackgroundImage;



    private void Start() // Récupération du script Fps_Character
    {
        if (player != null) player.GetComponent<Fps_Character>();
    }

    void Update() // Mise à jour de la barre de santé mentale
    {
        if (player == null) return;

        float sanity = player.PlayerSanityLevel;
        float maxsanity = player.PlayerSanityMax;
        if (maxsanity <= 0f) maxsanity = 1f;

        if (sanityBarImage != null)
            sanityBarImage.fillAmount = Mathf.Clamp01(sanity / maxsanity);


    }
}
