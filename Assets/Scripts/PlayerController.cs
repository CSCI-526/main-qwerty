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

    private bool isDead = false;
    private ulong playerId;

    public override void OnNetworkSpawn()
    {
        UpdateCurrentHealthRpc(maxHealth);
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    #region Health Methods
    [Rpc(SendTo.Owner)]
    private void UpdateCurrentHealthRpc(int newHealth)
    {
        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthBar.SetFillAmount((float)newHealth / maxHealth);
        if (currentHealth.Value <= 0)
        {
            isDead = true;
        }
    }
    
    public bool IsDead() { return isDead; }
    #endregion

    #region Network Variable Methods
    public void SetPlayerID(ulong id) => playerId = id;
    public ulong GetPlayerID() { return playerId; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
    #endregion
}
