using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

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
        go.transform.SetParent(playerPanel);
        PlayerController pc = go.GetComponent<PlayerController>();
        pc.SetPlayerID(requesterClientId);
        pc.SetPlayerName(playerName.ToString());
        pc.SetTargetingIdEveryoneRpc(requesterClientId);
        gameManager.AddPlayerRpc(pc.targetingId);
    }

    [Rpc(SendTo.Owner)]
    public void RequestSpawnEnemyIconOwnerRpc()
    {
        GameObject go = Instantiate(enemyPrefab.gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        go.transform.SetParent(enemyPanel);
        EnemyController ec = go.GetComponent<EnemyController>();
        ec.SetTargetingIdEveryoneRpc(++enemyIdCounter);
        gameManager.AddEnemyRpc(ec.targetingId);
    }
}
