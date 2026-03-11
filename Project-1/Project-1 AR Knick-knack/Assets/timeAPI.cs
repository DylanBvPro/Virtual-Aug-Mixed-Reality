using UnityEngine;
using TMPro;
using System;

public class TimeAPI : MonoBehaviour
{
    public GameObject timeTextObject;
    [SerializeField] private TMP_Text timeText;

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
        timeText.text = ohioNow.ToString("hh:mm:ss tt");
    }
}
