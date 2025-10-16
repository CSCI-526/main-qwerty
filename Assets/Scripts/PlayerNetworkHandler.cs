using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkHandler : NetworkBehaviour
{
    NetworkManager networkManager => NetworkManager.Singleton;
    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            FixedString128Bytes playerName = new FixedString128Bytes(networkManager.GetComponent<ConnectionManager>().GetProfileName());
            if (gameManager != null)
            {
                gameManager.SyncListsRpc(NetworkManager.Singleton.LocalClientId);
                gameManager.SpawnPlayer(NetworkManager.Singleton.LocalClientId, playerName.ToString());
                if (gameManager.IsOwner)
                    gameManager.SpawnEnemy();
            }
        }
    }
}
