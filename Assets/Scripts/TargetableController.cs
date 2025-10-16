using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TargetableController : NetworkBehaviour
{
    #region Health

    public int maxHealth = 100;
    
    [DoNotSerialize]
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public HealthBar healthBar;

    private bool isDead = false;

    protected virtual void InitHealth()
    {
        UpdateCurrentHealthRpc(maxHealth);
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    [Rpc(SendTo.Owner)]
    protected virtual void UpdateCurrentHealthRpc(int newHealth)
    {
        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    protected virtual void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBar != null)
            healthBar.SetFillAmount((float)newHealth / maxHealth);

        if (currentHealth.Value <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public virtual void ModifyCurrentHealth(int amount)
    {
        UpdateCurrentHealthRpc(currentHealth.Value + amount);
    }

    public bool IsDead() { return isDead; }

    protected abstract void Die();

    #endregion

    #region Targeting

    [DoNotSerialize]
    public NetworkVariable<FixedString128Bytes> targetWord = new NetworkVariable<FixedString128Bytes>(
        new FixedString128Bytes(""),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [DoNotSerialize]
    public NetworkVariable<ulong> networkedTargetingId = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public ulong targetingId;

    public TextMeshProUGUI targetWordText;

    [Rpc(SendTo.Owner)]
    protected virtual void UpdateTargetWordRpc(FixedString128Bytes newWord)
    {
        targetWord.Value = newWord;
    }

    protected virtual void InitTargeting()
    {
        OnTargetWordChanged(new FixedString128Bytes(""), targetWord.Value);
        targetWord.OnValueChanged += OnTargetWordChanged;
    }

    public virtual void SetTargetWord(string newWord)
    {
        UpdateTargetWordRpc(new FixedString128Bytes(newWord));
    }

    protected abstract void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord);

    public string GetTargetWord()
    {
        return targetWord.Value.ToString();
    }

    public void RandomizeTargetWord()
    {
        string newWord = gameManager.GenerateWord();
        SetTargetWord(newWord);
    }

    [Rpc(SendTo.Everyone)]
    public void SetTargetingIdEveryoneRpc(ulong id)
    {
        targetingId = id;
        if(IsOwner)
            networkedTargetingId.Value = id;
    }

    #endregion

    [DoNotSerialize]
    public GameManager gameManager => FindFirstObjectByType<GameManager>();
}
