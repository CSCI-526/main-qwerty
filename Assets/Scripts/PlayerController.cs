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

    [SerializeField] HealthBar healthBar;

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
}
