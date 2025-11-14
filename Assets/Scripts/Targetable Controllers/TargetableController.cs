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
    }

    [Rpc(SendTo.Owner)]
    protected virtual void UpdateCurrentHealthRpc(int newHealth)
    {
        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    protected virtual void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBar != null)
            healthBar.SetFillAmount(newHealth, maxHealth);

        if (currentHealth.Value <= 0)
        {
            if(IsOwner)
                isDead.Value = true;
            Die();
        }
    }

    public virtual void ModifyCurrentHealth(int amount)
    {
        UpdateCurrentHealthRpc(currentHealth.Value + amount);
    }

    public bool IsDead() { return isDead.Value; }

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
    }

    #endregion

    [DoNotSerialize]
    public GameManager gameManager => FindFirstObjectByType<GameManager>();
}
