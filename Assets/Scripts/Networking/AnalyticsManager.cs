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
        AnalyticsService.Instance.StartDataCollection();
    }

    public void StartAnalyticsDataCollection()
    {
        AnalyticsService.Instance.StartDataCollection();
    }

    public void PushAnalyticsEvent(CustomEvent analyticsEvent)
    {
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("Analytics Event Pushed: " + analyticsEvent.ToString());
    }
}
