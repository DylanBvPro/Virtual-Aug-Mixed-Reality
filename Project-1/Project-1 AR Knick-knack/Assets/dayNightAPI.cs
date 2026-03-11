using UnityEngine;
using TMPro;
using System;

public class dayNightAPI : MonoBehaviour
{
    public GameObject sunSphere;
    public GameObject moonSphere;
    [SerializeField] private TMP_Text timeText;

    void Start()
    {
        Debug.Log("Display Sun or Moon");

        if (timeText == null)
        {
            timeText = GetComponent<TMP_Text>();
        }

        UpdateOhioTime();
        InvokeRepeating(nameof(UpdateOhioTime), 0f, 1f);
    }

    private TimeZoneInfo GetOhioTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
        catch (System.TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        }
    }

    private void UpdateOhioTime()
    {
        TimeZoneInfo ohioTimeZone = GetOhioTimeZone();
        DateTime ohioNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ohioTimeZone);

        if (timeText != null)
        {
            timeText.text = ohioNow.ToString("hh:mm:ss tt");
        }

        UpdateSunMoonVisibility(ohioNow);
    }

    private void UpdateSunMoonVisibility(DateTime ohioNow)
    {
        Debug.Log("Current Ohio Time: " + ohioNow.Hour);
        bool isDayTime = ohioNow.Hour >= 7 && ohioNow.Hour < 18;
        Debug.Log("Is it day time? " + isDayTime);
        if (sunSphere != null)
        {
            sunSphere.SetActive(false);
        }

        if (moonSphere != null)
        {
            moonSphere.SetActive(false);
        }

        if (sunSphere != null)
        {
            sunSphere.SetActive(isDayTime);
        }

        if (moonSphere != null)
        {
            moonSphere.SetActive(!isDayTime);
        }
    }
}
