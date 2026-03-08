using UnityEngine;

public class TorchLightCameraFollow : MonoBehaviour
{
    public Transform target; // la caméra cible (si null, on prend Camera.main)
    [Tooltip("Activer le lissage pour un mouvement plus naturel")]
    public bool smoothFollow = true;
    public float positionLerpSpeed = 12f;
    public float rotationLerpSpeed = 20f;

    // États initiaux relatifs à la cible
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;
    private Transform cam;

    void Awake()
    {
        if (target == null && Camera.main != null)
            cam = Camera.main.transform;
        else
            cam = target;

        // Sauvegarde de la transformation initiale relative à la caméra
        if (cam != null)
        {
            initialLocalPosition = cam.InverseTransformPoint(transform.position);
            initialLocalRotation = Quaternion.Inverse(cam.rotation) * transform.rotation;
        }
        else
        {
            // fallback : garder les valeurs locales si aucune caméra
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
        }

        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 desiredWorldPos = cam.TransformPoint(initialLocalPosition);
        Quaternion desiredWorldRot = cam.rotation * initialLocalRotation;

        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, desiredWorldPos, 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredWorldRot, 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
        }
        else
        {
            transform.position = desiredWorldPos;
            transform.rotation = desiredWorldRot;
        }

        // Réappliquer l'échelle initiale pour éviter toute déformation par parent/scripting
        transform.localScale = initialLocalScale;
    }
}
