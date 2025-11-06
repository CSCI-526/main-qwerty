using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SharedCanvasController : NetworkBehaviour
{
    [SerializeField] private RectTransform playerPanel;
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private RectTransform enemyPanel;
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
        pc.SetPlayerIDRpc(requesterClientId);
        pc.SetPlayerName(playerName.ToString());
        pc.SetTargetingIdEveryoneRpc(requesterClientId);
        gameManager.AddPlayerRpc(pc.targetingId);
        RefreshLayoutGroupEveryoneRpc();
    }

    [Rpc(SendTo.Owner)]
    public void RequestSpawnEnemyIconOwnerRpc(float maxHealthMultiplier, float attackCooldownMultiplier, bool tutorialState)
    {
        GameObject go = Instantiate(enemyPrefab.gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        enemyPanel.GetComponent<CustomLayoutGroup>().AddToLayout(go.GetComponent<RectTransform>());
        EnemyController ec = go.GetComponent<EnemyController>();
        ec.SetTargetingIdEveryoneRpc(++enemyIdCounter);
        ec.SetMaxHealthRpc(maxHealthMultiplier);
        ec.SetAttackCooldown(attackCooldownMultiplier);
        ec.SetTutorial(tutorialState);
        gameManager.AddEnemyRpc(ec.targetingId);
        LayoutRebuilder.ForceRebuildLayoutImmediate(enemyPanel);
        RefreshLayoutGroupEveryoneRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void RefreshLayoutGroupEveryoneRpc()
    {
        playerPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
        enemyPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
    }
}
