using UnityEngine;

public class StatsEvent : Unity.Services.Analytics.Event
{
    public StatsEvent() : base("statsEvent")
    {
    }

    public int NumSubmissions { set {  SetParameter("numSubmissions", value); } }
    public float AverageWPM { set { SetParameter("averageWPM", value); } }
    public float AverageAccuracy { set { SetParameter("averageAccuracy", value); } }
    public int DamageDealt { set { SetParameter("damageDealt", value); } }
    public int HealingDone { set { SetParameter("healingDone", value); } }
}
