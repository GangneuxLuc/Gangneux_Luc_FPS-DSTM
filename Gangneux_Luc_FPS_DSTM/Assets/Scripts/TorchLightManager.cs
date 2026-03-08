using UnityEngine;

public class TorchLightManager : MonoBehaviour
{
    [Header("Torch Light Settings")]
    public float BatteryLife = 100f;
    public float BatteryMax = 100f;
    private float BatteryDrainRate = 2.5f;
    private Light torchLight;
    private bool lightIsOn = false;

    [Header("Audio Settings")]
    public AudioSource torchAudioSource;
    public AudioClip torchOnClip;
    public AudioClip torchOffClip;

    [Header("Audio Controls")]
    [Range(0f, 1f)] public float torchVolume = 1f; // volume global (inspector)
    [Range(0.5f, 2f)] public float torchPitch = 1f; // vitesse / pitch

    void Awake()
    {
        torchLight = GetComponent<Light>();
        if (torchAudioSource != null)
        {
            torchAudioSource.volume = torchVolume;
            torchAudioSource.pitch = torchPitch;
        }
    }

    void Update()
    {
        HandleLightToggle();
        Battery();
    }

    private void HandleLightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Toggled torch light");
            if (torchLight == null) return;

            // Appliquer volume et pitch avant de jouer un son
            if (torchAudioSource != null)
            {
                torchAudioSource.volume = torchVolume; // volume multiplié par PlayOneShot volumeScale
                torchAudioSource.pitch = torchPitch;   // change la vitesse & la hauteur
            }

            torchLight.enabled = !torchLight.enabled;

            // Utiliser PlayOneShot avec scale (optionnel)
            if (torchAudioSource != null && torchOnClip != null && torchLight.enabled)
                torchAudioSource.PlayOneShot(torchOnClip, 1f); // 1f = volumeScale (multiplie torchAudioSource.volume)
            else if (torchAudioSource != null && torchOffClip != null && !torchLight.enabled)
                torchAudioSource.PlayOneShot(torchOffClip, 1f);

            lightIsOn = torchLight.enabled;
        }
    }

    // Méthodes publiques pour régler dynamiquement depuis d'autres scripts / sliders UI
    public void SetTorchVolume(float v)
    {
        torchVolume = Mathf.Clamp01(v);
        if (torchAudioSource != null) torchAudioSource.volume = torchVolume;
    }

    public void SetTorchPitch(float p)
    {
        torchPitch = Mathf.Clamp(p, 0.5f, 2f);
        if (torchAudioSource != null) torchAudioSource.pitch = torchPitch;
    }

    private void Battery()
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
