using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var sharedCanvas = FindObjectOfType<SharedCanvasController>();
            if (sharedCanvas != null)
            {
                sharedCanvas.RequestSpawnIconOwnerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }
}
