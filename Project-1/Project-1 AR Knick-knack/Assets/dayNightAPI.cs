using UnityEngine;
using TMPro;
using System;

public class dayNightAPI : MonoBehaviour
{
    public GameObject sunSphere;
    public GameObject moonSphere;
    [SerializeField] private TMP_Text timeText;

    [Header("Testing Override")]
    public bool manualOverride = false;
    [Range(0, 23)] public int manualHour = 12;

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
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        }
    }

    private void UpdateOhioTime()
    {
        DateTime ohioNow;

        if (manualOverride)
        {
            ohioNow = DateTime.Today.AddHours(manualHour);
        }
        else
        {
            TimeZoneInfo ohioTimeZone = GetOhioTimeZone();
            ohioNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ohioTimeZone);
        }

        if (timeText != null)
        {
            timeText.text = ohioNow.ToString("hh:mm:ss tt");
        }

        UpdateSunMoonVisibility(ohioNow);
    }

    private void SetChildrenActive(GameObject parent, bool state)
    {
        if (parent == null) return;

        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }
    
    private void SetParticleSystems(GameObject parent, bool state)
    {
        if (parent == null) return;

        ParticleSystem[] particles = parent.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in particles)
        {
            var emission = ps.emission;
            emission.enabled = state;
        }
    }

    private void SetLights(GameObject parent, bool state)
    {
        if (parent == null) return;

        Light[] lights = parent.GetComponentsInChildren<Light>(true);

        foreach (Light l in lights)
        {
            l.enabled = state;
        }
    }

    private void UpdateSunMoonVisibility(DateTime ohioNow)
    {
        Debug.Log("Current Hour: " + ohioNow.Hour);

        bool isDayTime = ohioNow.Hour >= 7 && ohioNow.Hour < 18;

        // Enable/disable light components
        SetLights(sunSphere, isDayTime);
        SetLights(moonSphere, !isDayTime);

        // Particle Systems
        SetParticleSystems(sunSphere, isDayTime);
        SetParticleSystems(moonSphere, !isDayTime);

        // Also toggle the parent objects if you want
        if (sunSphere != null)
            sunSphere.SetActive(isDayTime);

        if (moonSphere != null)
            moonSphere.SetActive(!isDayTime);
    }

    public void SetDayMode()
    {
        manualOverride = true;
        manualHour = 12;
        UpdateOhioTime();
    }

    public void SetNightMode()
    {
        manualOverride = true;
        manualHour = 0;
        UpdateOhioTime();
    }

    public void ClearManualOverride()
    {
        manualOverride = false;
        UpdateOhioTime();
    }
}