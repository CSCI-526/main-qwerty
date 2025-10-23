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
}
