using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class SharedCanvasController : NetworkBehaviour
{
    [SerializeField] private TMP_Text sharedMessageText;
    [SerializeField] private RectTransform playerPanel;
    [SerializeField] private NetworkObject playerIconPrefab;

    private NetworkManager networkManager => NetworkManager.Singleton;

    private NetworkVariable<FixedString128Bytes> sharedMessage = new(
        "Welcome!",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

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

    public override void OnNetworkSpawn()
    {
        sharedMessage.OnValueChanged += (oldVal, newVal) =>
        {
            sharedMessageText.text = newVal.ToString();
        };

        UpdateMessageOwnerRpc(sharedMessage.Value.ToString());
    }

    [Rpc(SendTo.Owner)]
    public void UpdateMessageOwnerRpc(string message)
    {
        sharedMessage.Value = "";
        sharedMessage.Value = message;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(IsOwner)
            {
                UpdateMessageOwnerRpc("Hello from Owner!");
            }
            else
            {
                UpdateMessageOwnerRpc("Hello from Non Owner!");
            }
        }
    }
}
