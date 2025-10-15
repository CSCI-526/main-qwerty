using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField] PlayerIcon playerIcon;
    [SerializeField] HealthBar healthBar;

    private ulong playerId;

    public override void OnNetworkSpawn()
    {
        UpdateCurrentHealthRpc(maxHealth);
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    [Rpc(SendTo.Owner)]
    private void UpdateCurrentHealthRpc(int newHealth)
    {
        currentHealth.Value = newHealth;
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthBar.SetFillAmount((float)newHealth / maxHealth);
    }

    public void SetPlayerID(ulong id) => playerId = id;
    public ulong GetPlayerID() { return playerId; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
}
