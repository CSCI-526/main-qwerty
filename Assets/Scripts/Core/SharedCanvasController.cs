using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SharedCanvasController : NetworkBehaviour
{
    [SerializeField] public RectTransform playerPanel;
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] public RectTransform enemyPanel;
    [SerializeField] private List<NetworkObject> enemyPrefabs;

    private ulong enemyIdCounter = 0;

    private GameManager gameManager => FindFirstObjectByType<GameManager>();

    [Rpc(SendTo.Owner)]
    public void RequestSpawnPlayerIconOwnerRpc(ulong requesterClientId, FixedString128Bytes playerName)
    {
        GameObject go = Instantiate(playerPrefab.gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        playerPanel.GetComponent<CustomLayoutGroup>().AddToLayout(go.GetComponent<RectTransform>());
        PlayerController pc = go.GetComponent<PlayerController>();
        pc.SetPlayerID(requesterClientId);
        pc.SetPlayerName(playerName.ToString());
        pc.targetingID.Value = requesterClientId;
        gameManager.AddPlayer(new PlayerNetworkData
        {
            TargetingID = requesterClientId,
            PlayerName = playerName,
            IsReady = true
        });
        RefreshLayoutGroupEveryoneRpc();
    }

    [Rpc(SendTo.Owner)]
    public void RequestSpawnEnemyIconOwnerRpc(float maxHealthMultiplier, float attackCooldownMultiplier, bool tutorialState)
    {
        GameObject go = null;
        if (tutorialState)
            go = Instantiate(enemyPrefabs[0].gameObject);
        else
            go = Instantiate(GetRandomEnemyPrefab().gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        enemyPanel.GetComponent<CustomLayoutGroup>().AddToLayout(go.GetComponent<RectTransform>());
        EnemyController ec = go.GetComponent<EnemyController>();
        ec.targetingID.Value = ++enemyIdCounter;
        ec.SetMaxHealthRpc(maxHealthMultiplier);
        ec.SetAttackCooldownRpc(attackCooldownMultiplier);
        ec.SetTutorial(tutorialState);
        gameManager.AddEnemy(new EnemyNetworkData
        {
            TargetingID = enemyIdCounter,
            EnemyName = new FixedString128Bytes($"Enemy {enemyIdCounter}")
        });
        RefreshLayoutGroupEveryoneRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void RefreshLayoutGroupEveryoneRpc()
    {
        playerPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
        enemyPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
    }

    private NetworkObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[(int)Random.Range(0, enemyPrefabs.Count)];
    }
}
