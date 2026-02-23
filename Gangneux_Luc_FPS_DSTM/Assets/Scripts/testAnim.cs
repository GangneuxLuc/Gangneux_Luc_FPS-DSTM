using UnityEngine;

public class ApplyAnimationFromOther : MonoBehaviour
{
    [Header("Références")]
    public Animator sourceAnimator; // Animator du modèle qui a déjà l'animation
    public Animator targetAnimator; // Animator du modèle qui doit recevoir l'animation

    void Start()
    {
        if (sourceAnimator != null && targetAnimator != null)
        {
            // On copie le contrôleur d'animation
            targetAnimator.runtimeAnimatorController = sourceAnimator.runtimeAnimatorController;

            // Si les deux sont en Humanoid, on peut aussi copier l'Avatar
            if (sourceAnimator.avatar != null && sourceAnimator.avatar.isValid && sourceAnimator.avatar.isHuman)
            {
                targetAnimator.avatar = sourceAnimator.avatar;
            }

            Debug.Log("Animation appliquée avec succès !");
        }
        else
        {
            Debug.LogWarning("Veuillez assigner les deux Animator dans l'inspecteur.");
        }
    }
}
