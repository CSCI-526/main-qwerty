using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private void Start()
    {
        UnityServices.InitializeAsync();
    }

    public void SetPlayerConsent(bool consent)
    {
        if (consent)
            AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Starting Data Collection: " + consent);
    }

    public void PushAnalyticsEvent(StatsEvent analyticsEvent)
    {
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("Analytics Event Pushed: " + analyticsEvent.ToString());
    }
}
