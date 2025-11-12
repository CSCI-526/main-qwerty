using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    // Player Statistics
    private int numSubmissions = 0;
    private float averageWPM = 0f;
    private float averageAccuracy = 0f;
    private int damageDealt = 0;
    private int damageTaken = 0;
    private int healingDone = 0;

    // Ability Statistics
    private string className = "";
    private int ability1Uses = 0;
    private int ability2Uses = 0;
    private int ability3Uses = 0;
    private int ability4Uses = 0;

    // Curse/Buff Statistics
    private float damagePercentage = 1f;
    private float healingPercentage = 1f;
    private float punishmentPercentage = 1f;
    private float bulletSpeedPercentage = 1f;
    private string capitalizedCharacters = "";
    private string doubledCharacters = "";

    // Difficulty Statistics
    private int roundNumber = 0;
    private int enemyHealth = 0;
    private float enemyAttackSpeed = 0;
    private int numPlayers = 0;
    private int difficultyLevel = 0;

    private void Start()
    {
        UnityServices.InitializeAsync();
    }

    #region Unity Analytic Methods

    public void SetPlayerConsent(bool consent)
    {
        if (consent)
            AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Starting Data Collection: " + consent);
    }

    public void PushAnalyticsEvent()
    {
        BetaAnalyticsEvent analyticsEvent = CreateAnalyticsEvent();
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        AnalyticsService.Instance.Flush();

        resetPlayerStatistics();
        resetAbilityStatistics();
        resetCurseBuffStatistics();
        resetDifficultyStatistics();

        Debug.Log("Analytics Event Pushed: " + analyticsEvent.ToString());
    }

    public BetaAnalyticsEvent CreateAnalyticsEvent()
    {
        if (numSubmissions == 0)
            return null;

        BetaAnalyticsEvent analyticsEvent = new BetaAnalyticsEvent
        {
            NumSubmissions = numSubmissions,
            AverageWPM = averageWPM,
            AverageAccuracy = averageAccuracy,
            DamageDealt = damageDealt,
            DamageTaken = damageTaken,
            HealingDone = healingDone,
            ClassName = className,
            Ability1Uses = ability1Uses,
            Ability2Uses = ability2Uses,
            Ability3Uses = ability3Uses,
            Ability4Uses = ability4Uses,
            DamagePercentage = damagePercentage,
            HealingPercentage = healingPercentage,
            PunishmentPercentage = punishmentPercentage,
            BulletSpeedPercentage = bulletSpeedPercentage,
            CapitalizedCharacters = capitalizedCharacters,
            DoubledCharacters = doubledCharacters,
            RoundNumber = roundNumber,
            EnemyHealth = enemyHealth,
            EnemyAttackSpeed = enemyAttackSpeed,
            NumPlayers = numPlayers,
            DifficultyLevel = difficultyLevel
        };

        return analyticsEvent;
    }

    #endregion

    #region Statistics Modifiers/Resetters

    // Modifier Methods for Player Statistics
    public void addNumSubmissions(int submissions) { numSubmissions += submissions; }
    public void addAverageWPM(float wpm) { averageWPM = ((averageWPM * (numSubmissions - 1)) + wpm) / numSubmissions; }
    public void addAverageAccuracy(float accuracy) { averageAccuracy = ((averageAccuracy * (numSubmissions - 1)) + accuracy) / numSubmissions; }
    public void addDamageDealt(int damage) { damageDealt += damage; }
    public void addDamageTaken(int damage) { damageTaken += damage; }
    public void addHealingDone(int healing) { healingDone += healing; }

    public void resetPlayerStatistics()
    {
        numSubmissions = 0;
        averageWPM = 0f;
        averageAccuracy = 0f;
        damageDealt = 0;
        damageTaken = 0;
        healingDone = 0;
    }

    // Modifier Methods for Ability Statistics
    public void setClassName(string name) { className = name; }
    public void addAbility1Uses(int uses) { ability1Uses += uses; }
    public void addAbility2Uses(int uses) { ability2Uses += uses; }
    public void addAbility3Uses(int uses) { ability3Uses += uses; }
    public void addAbility4Uses(int uses) { ability4Uses += uses; }

    public void resetAbilityStatistics()
    {
        ability1Uses = 0;
        ability2Uses = 0;
        ability3Uses = 0;
        ability4Uses = 0;
    }

    // Modifier Methods for Curse/Buff Statistics
    public void setDamagePercentage(float percentage) { damagePercentage = percentage; }
    public void setHealingPercentage(float percentage) { healingPercentage = percentage; }
    public void setPunishmentPercentage(float percentage) { punishmentPercentage = percentage; }
    public void setBulletSpeedPercentage(float percentage) { bulletSpeedPercentage = percentage; }
    public void setCapitalizedCharacters(string characters) { capitalizedCharacters = characters; }
    public void addCapitalizedCharacter(char character) 
    { 
        if (!capitalizedCharacters.Contains(character.ToString()))
            capitalizedCharacters += character;
    }
    public void setDoubledCharacters(string characters) { doubledCharacters = characters; }
    public void addDoubledCharacter(char character) 
    { 
        if (!doubledCharacters.Contains(character.ToString()))
            doubledCharacters += character;
    }

    public void resetCurseBuffStatistics()
    {
        damagePercentage = 1f;
        healingPercentage = 1f;
        punishmentPercentage = 1f;
        bulletSpeedPercentage = 1f;
        capitalizedCharacters = "";
        doubledCharacters = "";
    }

    // Modifier Methods for Difficulty Statistics
    public void setRoundNumber(int round) { roundNumber = round; }
    public void setEnemyHealth(int health) { enemyHealth = health; }
    public void setEnemyAttackSpeed(float speed) { enemyAttackSpeed = speed; }
    public void setNumPlayers(int players) { numPlayers = players; }
    public void setDifficultyLevel(int level) { difficultyLevel = level; }

    public void resetDifficultyStatistics()
    {
        roundNumber = 0;
        enemyHealth = 0;
        enemyAttackSpeed = 0f;
        numPlayers = 0;
        difficultyLevel = 0;
    }

    #endregion

}
