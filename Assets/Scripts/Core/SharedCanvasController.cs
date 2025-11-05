using System.Collections;
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
    [SerializeField] private NetworkObject enemyPrefab;

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
        StartCoroutine(WaitForAddPlayer(requesterClientId, playerName.ToString()));
    }

    private IEnumerator WaitForAddPlayer(ulong targetingID, string playerName)
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.AddPlayer(new PlayerNetworkData
        {
            TargetingID = targetingID,
            PlayerName = playerName
        });
        RefreshLayoutGroupEveryoneRpc();
    }

    [Rpc(SendTo.Owner)]
    public void RequestSpawnEnemyIconOwnerRpc(float maxHealthMultiplier, float attackCooldownMultiplier)
    {
        GameObject go = Instantiate(enemyPrefab.gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        enemyPanel.GetComponent<CustomLayoutGroup>().AddToLayout(go.GetComponent<RectTransform>());
        EnemyController ec = go.GetComponent<EnemyController>();
        ec.targetingID.Value = ++enemyIdCounter;
        ec.SetMaxHealthRpc(maxHealthMultiplier);
        ec.SetAttackCooldown(attackCooldownMultiplier);
        StartCoroutine(WaitForAddEnemy(enemyIdCounter));
    }

    private IEnumerator WaitForAddEnemy(ulong targetingID)
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.AddEnemy(new EnemyNetworkData
        {
            TargetingID = targetingID,
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
}
