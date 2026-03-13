using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class WeatherAPI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject weatherTextObject;
    [SerializeField] private TMP_Text weatherText;

    [Header("Weather Objects")]
    public GameObject rainObject;
    public GameObject sunnyObject;
    public GameObject cloudyObject;
    public GameObject snowObject;

    [Header("Weather API Settings")]
    [SerializeField] private string apiKey = "54ee13e951f71304aa6f07db52079f66";
    [SerializeField] private float latitude = 38.5367471f;
    [SerializeField] private float longitude = -82.6829406f;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    [Header("Manual Override")]
    [Tooltip("Leave empty to use API weather. Options: rain, snow, sunny, cloudy")]
    public string manualWeather = "";

    private string lastKnownApiCondition = "unknown";
    private int lastKnownTempF = 0;
    private string currentCondition = "unknown";

    [System.Serializable]
    private class WeatherResponse
    {
        public MainData main;
        public WeatherData[] weather;
        public int cod;
        public string message;
    }

    [System.Serializable]
    private class MainData
    {
        public float temp;
    }

    [System.Serializable]
    private class WeatherData
    {
        public string main;
    }

    void Start()
    {
        if (weatherText == null)
        {
            if (weatherTextObject != null)
            {
                weatherText = weatherTextObject.GetComponent<TMP_Text>();
                if (weatherText == null)
                    weatherText = weatherTextObject.GetComponentInChildren<TMP_Text>(true);
            }

            if (weatherText == null)
                weatherText = GetComponent<TMP_Text>();
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("OpenWeather API key is missing. Add it in the WeatherAPI component Inspector.");
            return;
        }

        // Refresh weather every 15 minutes
        InvokeRepeating("GetDataFromWeb", 2f, 900f);
    }

    private string BuildWeatherUrl()
    {
        return $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={apiKey}&units=imperial";
    }

    void GetDataFromWeb()
    {
        StartCoroutine(GetRequest(BuildWeatherUrl()));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Weather API request failed ({webRequest.responseCode}): {webRequest.error}");
                Debug.LogError("Response body: " + webRequest.downloadHandler.text);
                yield break;
            }

            string json = webRequest.downloadHandler.text;
            WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(json);

            if (response == null || response.main == null)
            {
                Debug.LogError("Weather API JSON parse failed. Check API key and response format.");
                yield break;
            }

            int easyTempF = Mathf.RoundToInt(response.main.temp);
            lastKnownTempF = easyTempF;
            string apiCondition = (response.weather != null && response.weather.Length > 0)
                ? response.weather[0].main.ToLower()
                : "unknown";
            lastKnownApiCondition = apiCondition;

            // Decide the final condition: manual override takes precedence
            string finalCondition = string.IsNullOrWhiteSpace(manualWeather) ? apiCondition : manualWeather.ToLower();
            currentCondition = finalCondition;

            // Update weather text
            if (weatherText != null)
            {
                weatherText.text = easyTempF + "°F\n" + finalCondition;
            }

            // Update objects based on condition
            UpdateWeatherObjects(finalCondition);
        }
    }

    public void SetManualWeather(string condition)
    {
        manualWeather = string.IsNullOrWhiteSpace(condition) ? "" : condition.Trim().ToLower();
        ApplyCurrentCondition();
    }

    public void ClearManualWeather()
    {
        manualWeather = "";
        ApplyCurrentCondition();
    }

    private void ApplyCurrentCondition()
    {
        string finalCondition = string.IsNullOrWhiteSpace(manualWeather)
            ? lastKnownApiCondition
            : manualWeather;
        currentCondition = finalCondition;

        if (weatherText != null)
        {
            weatherText.text = lastKnownTempF + "°F\n" + finalCondition;
        }

        UpdateWeatherObjects(finalCondition);
    }

    public string GetCurrentCondition()
    {
        return currentCondition;
    }

    private void UpdateWeatherObjects(string condition)
    {
        // Disable all first
        if (rainObject != null) rainObject.SetActive(false);
        if (sunnyObject != null) sunnyObject.SetActive(false);
        if (cloudyObject != null) cloudyObject.SetActive(false);
        if (snowObject != null) snowObject.SetActive(false);

        // Enable objects depending on condition
        if (condition.Contains("rain") || condition.Contains("drizzle") || condition.Contains("thunderstorm"))
        {
            if (rainObject != null) rainObject.SetActive(true);
        }

        if (condition.Contains("snow"))
        {
            if (snowObject != null) snowObject.SetActive(true);
        }

        if (condition.Contains("sunny") || condition.Contains("clear"))
        {
            if (sunnyObject != null) sunnyObject.SetActive(true);
        }

        if (condition.Contains("clouds") || condition.Contains("cloudy"))
        {
            if (cloudyObject != null) cloudyObject.SetActive(true);
        }
    }
}