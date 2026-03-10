using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class WeatherAPI : MonoBehaviour
{
    public GameObject weatherTextObject;
    [SerializeField] private TMP_Text weatherText;

    [SerializeField] private string apiKey = "54ee13e951f71304aa6f07db52079f66";
    [SerializeField] private float latitude = 39.132061f;
    [SerializeField] private float longitude = -84.515541f;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello");

        if (weatherText == null)
        {
            if (weatherTextObject != null)
            {
                weatherText = weatherTextObject.GetComponent<TMP_Text>();
                if (weatherText == null)
                {
                    weatherText = weatherTextObject.GetComponentInChildren<TMP_Text>(true);
                }
            }

            if (weatherText == null)
            {
                weatherText = GetComponent<TMP_Text>();
            }
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("OpenWeather API key is missing. Add it in the WeatherAPI component Inspector.");
            return;
        }

        // wait a couple seconds to start and then refresh every 900 seconds

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
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Weather API request failed ({webRequest.responseCode}): {webRequest.error}");
                Debug.LogError("Response body: " + webRequest.downloadHandler.text);
                yield break;
            }

            string json = webRequest.downloadHandler.text;
            Debug.Log(":\nReceived: " + json);

            WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(json);
            if (response == null || response.main == null)
            {
                Debug.LogError("Weather API JSON parse failed. Check API key and response format.");
                yield break;
            }

            int easyTempF = Mathf.RoundToInt(response.main.temp);
            string conditions = (response.weather != null && response.weather.Length > 0)
                ? response.weather[0].main
                : "Unknown";

            if (weatherText == null)
            {
                Debug.LogError("No TMP_Text found. Assign Weather Text in the inspector, or set weatherTextObject to a GameObject that has (or contains) a TextMeshProUGUI/TextMeshPro component.");
                yield break;
            }

            weatherText.text = easyTempF + "°F\n" + conditions;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
