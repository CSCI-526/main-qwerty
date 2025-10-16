using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkHandler : NetworkBehaviour
{
    NetworkManager networkManager => NetworkManager.Singleton;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SharedCanvasController sharedCanvas = FindFirstObjectByType<SharedCanvasController>();
            FixedString128Bytes playerName = new FixedString128Bytes(networkManager.GetComponent<ConnectionManager>().GetProfileName());
            if (sharedCanvas != null)
            {
                sharedCanvas.RequestSpawnPlayerIconOwnerRpc(NetworkManager.Singleton.LocalClientId, playerName);
                sharedCanvas.RequestSpawnEnemyIconOwnerRpc();
            }
        }
    }
}
