using UnityEngine;
using TMPro;
using System;

public class TimeAPI : MonoBehaviour
{
    public GameObject timeTextObject;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private bool useManualHour = false;
    [SerializeField] private int manualHour24 = 12;

    void Start()
    {
        Debug.Log("Display Time");

        if (timeText == null)
        {
            if (timeTextObject != null)
            {
                timeText = timeTextObject.GetComponent<TMP_Text>();
                if (timeText == null)
                {
                    timeText = timeTextObject.GetComponentInChildren<TMP_Text>(true);
                }
            }

            if (timeText == null)
            {
                timeText = GetComponent<TMP_Text>();
            }
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
        if (timeText == null)
        {
            return;
        }

        TimeZoneInfo ohioTimeZone = GetOhioTimeZone();
        System.DateTime ohioNow = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.UtcNow, ohioTimeZone);

        if (useManualHour)
        {
            int clampedHour = Mathf.Clamp(manualHour24, 0, 23);
            ohioNow = new System.DateTime(
                ohioNow.Year,
                ohioNow.Month,
                ohioNow.Day,
                clampedHour,
                0,
                0
            );
        }

        timeText.text = ohioNow.ToString("hh:mm:ss tt");
    }

    public void SetManualHour24(int hour24)
    {
        useManualHour = true;
        manualHour24 = Mathf.Clamp(hour24, 0, 23);
        UpdateOhioTime();
    }

    public void SetDayMode()
    {
        SetManualHour24(12);
    }

    public void SetNightModeHour24()
    {
        // "24:00" maps to 00:00 in DateTime.
        SetManualHour24(0);
    }

    public void ClearManualTime()
    {
        useManualHour = false;
        UpdateOhioTime();
    }
}
