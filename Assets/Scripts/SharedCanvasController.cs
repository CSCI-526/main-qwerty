using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class SharedCanvasController : NetworkBehaviour
{
    [SerializeField] private RectTransform playerPanel;
    [SerializeField] private NetworkObject playerIconPrefab;

    private NetworkManager networkManager => NetworkManager.Singleton;

    [Rpc(SendTo.Owner)]
    public void RequestSpawnIconOwnerRpc(ulong requesterClientId, FixedString128Bytes playerName)
    {
        GameObject go = Instantiate(playerIconPrefab.gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn(true);
        go.transform.SetParent(playerPanel);
        PlayerController pc = go.GetComponent<PlayerController>();
        pc.SetPlayerID(requesterClientId);
        pc.SetPlayerName(playerName.ToString());
    }
}
