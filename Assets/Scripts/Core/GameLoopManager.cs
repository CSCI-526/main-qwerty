using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameLoopManager : NetworkBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject typingElements;
    [SerializeField] private GameObject startBattleButton;
    [SerializeField] private GameObject notReadyWarning;
    [SerializeField] private GameObject cursePanel;

    private bool tutorial = true;
    private bool tutorialStage = true;
    private bool inCombat = false;
    private int battleCount = 0;

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public override void OnNetworkSpawn()
    {
        ToggleElementsRpc(false, true, false, tutorial);
    }

    private void Update()
    {
        if (gameManager.localPlayer != null && gameManager.localPlayer.IsDead())
        {
            typingElements.SetActive(false);
        }
        if (!IsOwner) return;
        if (inCombat && gameManager.IsEnemiesDead())
        {
            EndBattle();
        }
        else if (inCombat && gameManager.IsPlayersDead())
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        if (!IsOwner) return;
        inCombat = false;
        gameManager.RemoveAllPlayers();
        gameManager.RemoveAllEnemies();
        gameManager.RemoveAllProjectiles();
        battleCount = 0;
        ResetCurses();
        ToggleElementsRpc(false, true, false);
    }

    public void CreatePlayers()
    {
        if (!IsOwner) return;
        if (gameManager.PlayersSpawned()) return;
        gameManager.RemoveAllPlayers();
        foreach (ulong clientID in gameManager.networkManager.ConnectedClientsIds)
        {
            SpawnPlayerFromClientRpc(RpcTarget.Single(clientID, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SpawnPlayerFromClientRpc(RpcParams rpcParams)
    {
        gameManager.SpawnPlayer(gameManager.networkManager.LocalClientId, gameManager.networkManager.GetComponent<ConnectionManager>().GetProfileName());
    }

    [Rpc(SendTo.Owner)]
    public void StartBattleRpc()
    {
        if (inCombat) return;
        if (!gameManager.AllReady())
        {
            StartCoroutine(ShowNotReadyWarning(3f));
        }
        else
            StartCoroutine(StartBattle());
    }

    private IEnumerator ShowNotReadyWarning(float delay)
    {
        notReadyWarning.SetActive(true);
        yield return new WaitForSeconds(delay);
        notReadyWarning.SetActive(false);
    }

    public IEnumerator StartBattle()
    {
        CreatePlayers();
        gameManager.SpawnEnemy();
        ToggleElementsRpc(true, false, false, tutorial);
        yield return new WaitForSeconds(2f);
        inCombat = true;
        gameManager.ResetReadyStatusRpc();
        ShareRoundStats();
    }

    public void EndBattle()
    {
        if (!inCombat || !IsOwner) return;
        inCombat = false;
        gameManager.RemoveAllEnemies();
        gameManager.RemoveAllProjectiles();
        battleCount++;
        SendAnalyticsRpc();
        AssignRandomCurses();
        ToggleElementsRpc(false, false, true);
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleElementsRpc(bool typingElementsState, bool startBattleButtonState, bool cursePanelState, bool tutorialState = false)
    {
        typingElements.SetActive(typingElementsState);
        if(IsOwner)
            startBattleButton.SetActive(startBattleButtonState);
        cursePanel.SetActive(cursePanelState);
        if (tutorialState)
        {
            tutorialStage = true;
        }
        else
        {
            tutorial = false;
            tutorialStage = false;
        }
    }

    public void ResetCurses()
    {
        if (!IsOwner) return;
        foreach (ulong clientID in gameManager.networkManager.ConnectedClientsIds)
        {
            gameManager.ResetCurseBuffEffectRpc(RpcTarget.Single(clientID, RpcTargetUse.Temp));
        }
    }

    public void AssignRandomCurses()
    {
        if (!IsOwner) return;
        foreach (ulong clientID in gameManager.networkManager.ConnectedClientsIds)
        {
            gameManager.AddRandomCurseBuffEffectRpc(RpcTarget.Single(clientID, RpcTargetUse.Temp));
        }
    }

    public float GetEnemyHealthMultiplier()
    {
        if (!IsOwner) return 0f;
        return (1f + (battleCount * 0.4f)) * (1f + ((gameManager.networkManager.ConnectedClients.Count - 1) * 0.3f));
    }

    public float GetEnemyAttackCooldownMultiplier()
    {
        if (!IsOwner) return 0f;
        return Mathf.Pow(0.9f, battleCount) * Mathf.Pow(0.8f, gameManager.networkManager.ConnectedClients.Count - 1);
    }

    public bool GetTutorialState()
    {
        return tutorialStage;
    }

    private void ShareRoundStats()
    {
        if (!IsOwner) return;
        ShareRoundStatsRpc(battleCount);
    }

    [Rpc(SendTo.Everyone)]
    private void ShareRoundStatsRpc(int roundNumber)
    {
        gameManager.analyticsManager.setRoundNumber(roundNumber);
        gameManager.analyticsManager.setEnemyHealth(FindFirstObjectByType<EnemyController>().maxHealth);
        gameManager.analyticsManager.setEnemyAttackSpeed(FindFirstObjectByType<EnemyController>().attackCooldown);
        gameManager.analyticsManager.setNumPlayers(gameManager.networkManager.ConnectedClients.Count);
        gameManager.analyticsManager.setDifficultyLevel(1);

        TypingEffectManager.TypingStats stats = gameManager.typingEffectManager.ReportStats();
        if (stats != null)
        {
            gameManager.analyticsManager.setDamagePercentage(stats.damagePercentage);
            gameManager.analyticsManager.setHealingPercentage(stats.healingPercentage);
            gameManager.analyticsManager.setPunishmentPercentage(stats.punishmentPercentage);
            gameManager.analyticsManager.setBulletSpeedPercentage(stats.bulletSpeedPercentage);
            gameManager.analyticsManager.setCapitalizedCharacters(stats.capitalizedCharacters);
            gameManager.analyticsManager.setDoubledCharacters(stats.doubledCharacters);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SendAnalyticsRpc()
    {
        StartCoroutine(WaitAnalyticsCollection());
    }

    private IEnumerator WaitAnalyticsCollection()
    {
        yield return new WaitForSeconds(2f);
        gameManager.analyticsManager.PushAnalyticsEvent();
    }
}
