using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkHandler : NetworkBehaviour
{
    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (gameManager != null)
            {
                gameManager.SyncListsRpc(gameManager.networkManager.LocalClientId);
            }
        }
    }
}
