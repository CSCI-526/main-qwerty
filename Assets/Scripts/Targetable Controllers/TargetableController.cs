using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

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

    [DoNotSerialize]
    public NetworkVariable<int> currentShield = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public HealthBar healthBar;

    protected NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    protected virtual void InitHealth()
    {
        UpdateCurrentHealthRpc(maxHealth);
        OnHealthChanged(currentHealth.Value, maxHealth);
        currentHealth.OnValueChanged += OnHealthChanged;
        currentShield.OnValueChanged += OnShieldChanged;
    }

    [Rpc(SendTo.Owner)]
    protected virtual void UpdateCurrentHealthRpc(int changeAmount)
    {
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + changeAmount, 0, maxHealth);
    }

    protected virtual void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBar != null)
            healthBar.SetFillAmount(newHealth, currentShield.Value, maxHealth);

        if (currentHealth.Value <= 0)
        {
            if(IsOwner)
                isDead.Value = true;
            Die();
        }
    }

    protected virtual void OnShieldChanged(int oldShield, int newShield)
    {
        if (healthBar != null && isDead.Value == false)
            healthBar.SetFillAmount(currentHealth.Value, newShield, maxHealth);
    }

    public virtual void ModifyCurrentHealth(int amount)
    {
        if (amount >= 0)
        {
            UpdateCurrentHealthRpc(amount);
        }
        else if (currentShield.Value > 0)
        {
            int temp = amount;
            amount += currentShield.Value;
            ModifyCurrentShieldRpc(temp);
        }

        if (amount < 0)
        {
            UpdateCurrentHealthRpc(amount);
        }
    }

    [Rpc(SendTo.Owner)]
    public virtual void ModifyCurrentShieldRpc(int amount)
    {
        currentShield.Value = Mathf.Clamp(currentShield.Value + amount, 0, int.MaxValue);
    }

    public bool IsDead() { return isDead.Value; }

    protected abstract void Die();

    [Rpc(SendTo.Owner)]
    public void ReviveRpc()
    {
        isDead.Value = false;
    }

    #endregion

    #region Targeting

    [DoNotSerialize]
    public NetworkVariable<FixedString128Bytes> targetWord = new NetworkVariable<FixedString128Bytes>(
        new FixedString128Bytes(""),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [DoNotSerialize]
    public NetworkVariable<ulong> targetingID = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

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
        targetingID.OnValueChanged += OnTargetIDChanged;
        OnTargetWordChanged(new FixedString128Bytes(""), targetWord.Value);
        OnTargetIDChanged(ulong.MaxValue, targetingID.Value);
    }

    public virtual void SetTargetWord(string newWord)
    {
        UpdateTargetWordRpc(new FixedString128Bytes(newWord));
    }

    protected abstract void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord);

    protected abstract void OnTargetIDChanged(ulong oldID, ulong newID);

    public string GetTargetWord()
    {
        return targetWord.Value.ToString();
    }

    public void RandomizeTargetWord()
    {
        string newWord = gameManager.GenerateWord();
        SetTargetWord(newWord);
    }

    #endregion

    #region Buff/Debuff

    [SerializeField] protected GameObject attackBuff;
    [SerializeField] protected GameObject damageDebuff;
    [SerializeField] protected GameObject leechBuff;
    [SerializeField] protected CustomLayoutGroup buffDebuffLayoutGroup;
    [SerializeField] public GameObject effectTarget;

    public struct BuffDebuffData {
        public float modifier;
        public int duration;
        public FixedString128Bytes effectType;

        public void assignBuffDebuffData(float Modifier, int Duration, FixedString128Bytes EffectType)
        {
            modifier = Modifier;
            duration = Duration;
            effectType = EffectType;
        }
    }

    List<BuffDebuffData> BuffDebuffList = new List<BuffDebuffData>();

    public float calculateDamageModifier()
    {
        float modifier = 1.0f;
        for (int i = BuffDebuffList.Count - 1; i >= 0; i--)
        {
            var effect = BuffDebuffList[i];
            if (effect.effectType.ToString().Equals("DamageBuff"))
            {
                modifier *= effect.modifier;
                effect.duration--;
                if (effect.duration <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                    RefreshBuffDebuffUI();
                }
                else
                {
                    BuffDebuffList[i] = effect; // write back the mutated struct
                }
            }
        }
        return modifier;
    }

    public float calculateHealModifier()
    {
        float modifier = 1.0f;
        for (int i = BuffDebuffList.Count - 1; i >= 0; i--)
        {
            var effect = BuffDebuffList[i];
            if (effect.effectType.ToString().Equals("HealBuff"))
            {
                modifier *= effect.modifier;
                effect.duration--;
                if (effect.duration <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                }
                else
                {
                    BuffDebuffList[i] = effect; // write back the mutated struct
                }
            }
        }
        return modifier;
    }

    public float calculateLeechModifier()
    {
        float modifier = 0.0f;
        for (int i = BuffDebuffList.Count - 1; i >= 0; i--)
        {
            var effect = BuffDebuffList[i];
            if (effect.effectType.ToString().Equals("LeechBuff"))
            {
                modifier += effect.modifier;
                effect.duration--;
                if (effect.duration <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                    RefreshBuffDebuffUI();
                }
                else
                {
                    BuffDebuffList[i] = effect;
                }
            }
        }
        return modifier;
    }

    public float calculateDamageTakenModifier()
    {
        float modifier = 1.0f;
        for (int i = BuffDebuffList.Count - 1; i >= 0; i--)
        {
            var effect = BuffDebuffList[i];
            if (effect.effectType.ToString().Equals("DamageTakenDebuff"))
            {
                modifier *= (float)effect.modifier;
                effect.duration--;
                if (effect.duration <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                    RefreshBuffDebuffUI();
                }
                else
                {
                    BuffDebuffList[i] = effect;
                }
            }
        }
        return modifier;
    }

    public void AddBuffDebuff(float modifier, int duration, FixedString128Bytes effectType)
    {
        BuffDebuffData data = new BuffDebuffData
        {
            modifier = modifier,
            duration = duration,
            effectType = effectType
        };
        BuffDebuffList.Add(data);
        RefreshBuffDebuffUI();
    }

    public void RefreshBuffDebuffUI()
    {
        if(attackBuff == null || damageDebuff == null || leechBuff == null || buffDebuffLayoutGroup == null)
        {
            return;
        }

        SetAttackBuffRpc(false);
        SetDamageDebuffRpc(false);
        SetLeechBuffRpc(false);

        foreach (BuffDebuffData data in BuffDebuffList)
        {
            if (data.effectType.ToString().Equals("DamageBuff"))
            {
                SetAttackBuffRpc(true);
            }
            else if (data.effectType.ToString().Equals("DamageTakenDebuff"))
            {
                SetDamageDebuffRpc(true);
            }
            else if (data.effectType.ToString().Equals("LeechBuff"))
            {
                SetLeechBuffRpc(true);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetAttackBuffRpc(bool state)
    {
        attackBuff.SetActive(state);
        buffDebuffLayoutGroup.RefreshLayout();
    }

    [Rpc(SendTo.Everyone)]
    public void SetDamageDebuffRpc(bool state)
    {
        damageDebuff.SetActive(state);
        buffDebuffLayoutGroup.RefreshLayout();
    }

    [Rpc(SendTo.Everyone)]
    public void SetLeechBuffRpc(bool state)
    {
        leechBuff.SetActive(state);
        buffDebuffLayoutGroup.RefreshLayout();
    }

    #endregion

    [DoNotSerialize]
    public GameManager gameManager => FindFirstObjectByType<GameManager>();
}
