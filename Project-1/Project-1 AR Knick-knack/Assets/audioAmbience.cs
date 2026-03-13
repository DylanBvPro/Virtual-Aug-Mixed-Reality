using UnityEngine;

public class AmbientAudioPlayer : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Weather Source")]
    [SerializeField] private WeatherAPI weatherAPI;

    [Header("Assign clips from Assets/SceneAssets/Sounds")]
    [SerializeField] private AudioClip rainyClip;
    [SerializeField] private AudioClip cloudyClip;
    [SerializeField] private AudioClip defaultClip;

    private string lastAudioCategory = "";

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (weatherAPI == null)
        {
            weatherAPI = FindFirstObjectByType<WeatherAPI>();
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AmbientAudioPlayer needs an AudioSource.");
            return;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        UpdateAmbientAudio(true);
    }

    private void Update()
    {
        UpdateAmbientAudio(false);
    }

    private void UpdateAmbientAudio(bool forceRefresh)
    {
        if (audioSource == null)
        {
            return;
        }

        string currentCondition = weatherAPI != null ? weatherAPI.GetCurrentCondition() : "unknown";
        string newCategory = GetAudioCategory(currentCondition);

        if (!forceRefresh && newCategory == lastAudioCategory)
        {
            return;
        }

        AudioClip nextClip = GetClipForCategory(newCategory);
        if (nextClip == null)
        {
            return;
        }

        if (audioSource.clip != nextClip)
        {
            audioSource.clip = nextClip;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (forceRefresh || newCategory != lastAudioCategory)
        {
            audioSource.Stop();
            audioSource.Play();
        }

        lastAudioCategory = newCategory;
    }

    private string GetAudioCategory(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return "default";
        }

        string normalizedCondition = condition.ToLower();

        if (normalizedCondition.Contains("rain") || normalizedCondition.Contains("drizzle") || normalizedCondition.Contains("thunderstorm"))
        {
            return "rainy";
        }

        if (normalizedCondition.Contains("cloud"))
        {
            return "cloudy";
        }

        return "default";
    }

    private AudioClip GetClipForCategory(string category)
    {
        switch (category)
        {
            case "rainy":
                return rainyClip != null ? rainyClip : defaultClip;

            case "cloudy":
                return cloudyClip != null ? cloudyClip : defaultClip;

            default:
                return defaultClip;
        }
    }
}