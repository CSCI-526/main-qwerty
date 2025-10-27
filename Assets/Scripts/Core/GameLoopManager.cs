using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameLoopManager : NetworkBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject typingElements;
    [SerializeField] private GameObject startBattleButton;

    private bool inCombat = false;
    private int battleCount = 0;

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public override void OnNetworkSpawn()
    {
        ToggleTypingElementsRpc(false);
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
        ToggleTypingElementsRpc(false);
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
        startBattleButton.SetActive(false);
        yield return new WaitForSeconds(2f);
        ToggleTypingElementsRpc(true);
        inCombat = true;
    }

    public void EndBattle()
    {
        if (!inCombat || !IsOwner) return;
        inCombat = false;
        gameManager.RemoveAllEnemiesRpc();
        gameManager.RemoveAllProjectilesRpc();
        ToggleTypingElementsRpc(false);
        battleCount++;
        AssignRandomCurses();
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleTypingElementsRpc(bool state)
    {
        typingElements.SetActive(state);
        if(IsOwner)
            startBattleButton.SetActive(!state);

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
        return (1f + (battleCount * 0.4f)) * (1f + ((gameManager.networkManager.ConnectedClients.Count - 1) * 0.2f));
    }

    public float GetEnemyAttackCooldownMultiplier()
    {
        if (!IsOwner) return 0f;
        return Mathf.Pow(0.9f, battleCount) * Mathf.Pow(0.85f, gameManager.networkManager.ConnectedClients.Count - 1);
    }
}
