using UnityEngine;

public class BetaAnalyticsEvent : Unity.Services.Analytics.Event
{
    public BetaAnalyticsEvent() : base("betaAnalyticsEvent")
    {
    }

    // Player Statistics
    public int NumSubmissions { set { SetParameter("numSubmissions", value); } }
    public float AverageWPM { set { SetParameter("averageWPM", value); } }
    public float AverageAccuracy { set { SetParameter("averageAccuracy", value); } }
    public int DamageDealt { set { SetParameter("damageDealt", value); } }
    public int DamageTaken { set { SetParameter("damageTaken", value); } }
    public int HealingDone { set { SetParameter("healingDone", value); } }

    // Ability Statistics
    public string ClassName { set { SetParameter("className", value); } }
    public int Ability1Uses { set { SetParameter("ability1Uses", value); } }
    public int Ability2Uses { set { SetParameter("ability2Uses", value); } }
    public int Ability3Uses { set { SetParameter("ability3Uses", value); } }
    public int Ability4Uses { set { SetParameter("ability4Uses", value); } }

    // Curse/Buff Statistics
    public float DamagePercentage { set { SetParameter("damagePercentage", value); } }
    public float HealingPercentage { set { SetParameter("healingPercentage", value); } }
    public float PunishmentPercentage { set { SetParameter("punishmentPercentage", value); } }
    public float BulletSpeedPercentage { set { SetParameter("bulletSpeedPercentage", value); } }
    public string CapitalizedCharacters { set { SetParameter("capitalizedCharacters", value); } }
    public string DoubledCharacters { set { SetParameter("doubledCharacters", value); } }

    // Difficulty Statistics
    public int RoundNumber { set { SetParameter("roundNumber", value); } }
    public int EnemyHealth { set { SetParameter("enemyHealth", value); } }
    public float EnemyAttackSpeed { set { SetParameter("enemyAttackSpeed", value); } }
    public int NumPlayers { set { SetParameter("numPlayers", value); } }
    public int DifficultyLevel { set { SetParameter("difficultyLevel", value); } }

}
