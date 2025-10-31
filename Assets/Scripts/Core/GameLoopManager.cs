using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameLoopManager : NetworkBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject typingElements;
    [SerializeField] private GameObject startBattleButton;
    [SerializeField] private GameObject cursePanel;

    private bool inCombat = false;
    private int battleCount = 0;

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public override void OnNetworkSpawn()
    {
        ToggleElementsRpc(false, true, false);
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
        gameManager.RemoveAllPlayersRpc();
        gameManager.RemoveAllEnemiesRpc();
        gameManager.RemoveAllProjectilesRpc();
        battleCount = 0;
        ToggleElementsRpc(false, true, false);
    }

    public void CreatePlayers()
    {
        if (!IsOwner) return;
        if (gameManager.PlayersSpawned()) return;
        gameManager.RemoveAllPlayersRpc();
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
        if(inCombat) return;
        StartCoroutine(StartBattle());
    }

    public IEnumerator StartBattle()
    {
        CreatePlayers();
        gameManager.SpawnEnemy();
        yield return new WaitForSeconds(2f);
        ToggleElementsRpc(true, false, false);
        inCombat = true;
    }

    public void EndBattle()
    {
        if (!inCombat || !IsOwner) return;
        inCombat = false;
        gameManager.RemoveAllEnemiesRpc();
        gameManager.RemoveAllProjectilesRpc();
        battleCount++;
        AssignRandomCurses();
        ToggleElementsRpc(false, false, true);
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleElementsRpc(bool typingElementsState, bool startBattleButtonState, bool cursePanelState)
    {
        typingElements.SetActive(typingElementsState);
        if(IsOwner)
            startBattleButton.SetActive(startBattleButtonState);
        cursePanel.SetActive(cursePanelState);
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
}
