using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class TypingEffectManager : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText; // list of current curses & buffs
    [SerializeField] private GameObject effectPanel;
    [SerializeField] private float modMultiplier = 1.2f;

    private List<TypingEffectBase> activeTypingEffects = new(); // currently active curses & buffs

    private void Start()
    {
        activeTypingEffects = new();
        UpdateEffectText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            effectPanel.SetActive(!effectPanel.activeSelf);
        }
    }

    private void OnEffectChange()
    {
        UpdateEffectText();
    }

    private void UpdateEffectText()
    {
        if (effectText != null)
        {
            List<string> curseDescs = new();
            List<string> buffDescs = new();
            int bulletSpeedModTotal = 0;
            int damageModTotal = 0;
            int healModTotal = 0;
            int punishmentModTotal = 0;

            foreach (var activeTypingEffect in activeTypingEffects)
            {
                int bulletSpeedMod = activeTypingEffect.ApplyBulletSpeedMod();
                int damageMod = activeTypingEffect.ApplyDamageMod();
                int healMod = activeTypingEffect.ApplyHealMod();
                int punishmentMod = activeTypingEffect.ApplyPunishmentMod();
                if (bulletSpeedMod != 0 || damageMod != 0 || healMod != 0 || punishmentMod != 0)
                {
                    bulletSpeedModTotal += bulletSpeedMod;
                    damageModTotal += damageMod;
                    healModTotal += healMod;
                    punishmentModTotal += punishmentMod;
                }
                else
                {
                    if (activeTypingEffect.IsCurse())
                    {
                        curseDescs.Add(activeTypingEffect.GetEffectDescription());
                    }
                    else
                    {
                        buffDescs.Add(activeTypingEffect.GetEffectDescription());
                    }
                }
            }

            float bulletSpeedModPercentage = (float)Math.Pow(modMultiplier, bulletSpeedModTotal) * 100f;
            float healModPercentage = 1.0f / (float)Math.Pow(modMultiplier, healModTotal) * 100f;
            float damageModPercentage = 1.0f / (float)Math.Pow(modMultiplier, damageModTotal) * 100f;
            float punishmentModPercentage = (float)Math.Pow(modMultiplier, punishmentModTotal) * 100f;

            string desc = $"{damageModPercentage:F2}% damage, {healModPercentage:F2}% healing, \n{punishmentModPercentage:F2}% punishment, {bulletSpeedModPercentage:F2}% enemy bullet speed\n";
            desc += "Current Curses:\n" + string.Join(", ", curseDescs) + "\n";
            desc += "Current Buffs:\n" + string.Join(", ", buffDescs) + "\n";
            effectText.text = desc;
        }
    }

    public void ResetTypingEffects()
    {
        activeTypingEffects = new();
        UpdateEffectText();
    }

    /// <summary>
    /// Apply active effects to the prompt. Effects are applied per-prompt instead of per-character.
    /// </summary>
    /// <param name="prompt">The original prompt (also the displayed prompt).</param>
    /// <returns>The underlying prompt used for comparing with user inputs.</returns>
    public string ApplyEffectOnPrompt(ref string prompt)
    {
        string newPrompt = prompt;
        foreach (var typingEffect in activeTypingEffects)
        {
            newPrompt = typingEffect.ApplyEffectOnPrompt(ref newPrompt);
        }
        return newPrompt;
    }

    /// <summary>
    /// Apply active effects to the mod. Effects are applied to the game.
    /// </summary>
    /// <returns>The mod containing punishment, heal and damages.</returns>
    public int[] ApplyEffectOnMod()
    {
        int[] output = { 0, 0, 0, 0 };
        foreach (var typingEffect in activeTypingEffects)
        {
            if (typingEffect.ApplyPunishmentMod() != 0)
            {
                output[0] += typingEffect.ApplyPunishmentMod();
            }

            if (typingEffect.ApplyHealMod() != 0)
            {
                output[1] += typingEffect.ApplyHealMod();
            }

            if (typingEffect.ApplyDamageMod() != 0)
            {
                output[2] += typingEffect.ApplyDamageMod();
            }

            if (typingEffect.ApplyBulletSpeedMod() != 0)
            {
                output[3] += typingEffect.ApplyBulletSpeedMod();
            }
        }

        return output;
    }

    /// <summary>
    /// Apply some effects (e.g. autocorrect quota) to error count (per-prompt). 
    /// </summary>
    /// <param name="errors">Reference to error counter.</param>
    public void OnEndTyping(ref int errors)
    {
        foreach (var typingEffect in activeTypingEffects)
        {
            typingEffect.OnEndTyping(ref errors);
        }
    }

    /// <summary>
    /// Interface for creating effect.
    /// </summary>
    /// <param name="typingEffect">The effect to be added.</param>
    public void AddTypingEffect(TypingEffectBase typingEffect)
    {
        if (!activeTypingEffects.Contains(typingEffect))
        {
            activeTypingEffects.Add(typingEffect);
            OnEffectChange();
        }
    }

    /// <summary>
    /// Interface for removing curses (mainly) & (some temporary) buffs 
    /// </summary>
    /// <param name="typingEffect">The effect to be removed.</param>
    public void RemoveTypingEffect(TypingEffectBase typingEffect)
    {
        if (activeTypingEffects.Contains(typingEffect))
        {
            activeTypingEffects.Remove(typingEffect);
            OnEffectChange();
        }
    }

    /// <summary>
    /// Shorthand for creating ForceCapitalize Curse.
    /// </summary>
    /// <param name="letter">The letter that is forced capitalized.</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase ForceCapitalize(char letter)
    {
        var effect = ScriptableObject.CreateInstance<ForceCapitalizeCurseData>();
        effect.Initialize(letter);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// Shorthand for creating ForceDoubling Curse.
    /// </summary>
    /// <param name="letter">The letter that is forced doubled.</param>
    /// <param name="isCaseSensitive">Is the provided letter case sensitive or not. (case-insensitive by default)</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase ForceDoubling(char letter, bool isCaseSensitive = false)
    {
        var effect = ScriptableObject.CreateInstance<ForceDoublingCurseData>();
        effect.Initialize(letter, isCaseSensitive);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// Shorthand for creating AllLowercase Buff (for all letters).
    /// </summary>
    /// <returns>Effect data</returns>
    public TypingEffectBase AllLowercase()
    {
        var effect = ScriptableObject.CreateInstance<AllLowercaseBuffData>();
        // effect.Initialize();
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// (Deprecated) Shorthand for creating DisableLetter Curse.
    /// </summary>
    /// <param name="letter">The letter to be disabled (case sensitive or not).</param>
    /// <param name="isCaseSensitive">Is the provided letter case sensitive or not. (case-insensitive by default)</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase DisableLetter(char letter, bool isCaseSensitive = false)
    {
        var effect = ScriptableObject.CreateInstance<DisableLetterCurseData>();
        effect.Initialize(letter, isCaseSensitive);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// (Deprecated) Shorthand for creating AutoCorrect Buff.
    /// </summary>
    /// <param name="count">The autocorrect quota to be added.</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase AutoCorrect(int count)
    {
        var effect = ScriptableObject.CreateInstance<AutoCorrectBuffData>();
        effect.Initialize(count);
        // AddTypingEffect(effect);
        return effect;
    }


    /// <summary>
    /// Shorthand for creating Punishment Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - take double damage) or buff (-1 - take half damage).</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase PunishmentMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(curse, 0, 0, 0);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// Shorthand for creating Damage Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - damage halved) or buff (-1 - damage doubled).</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase DamageMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(0, 0, curse, 0);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// Shorthand for creating Heal Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - heal halved) or buff (-1 - heal doubled).</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase HealMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(0, curse, 0, 0);
        // AddTypingEffect(effect);
        return effect;
    }

    /// <summary>
    /// Shorthand for creating Bullet Speed Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - speed doubled) or buff (-1 - speed halved).</param>
    /// <returns>Effect data</returns>
    public TypingEffectBase BulletSpeedMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(0, 0, 0, curse);
        // AddTypingEffect(effect);
        return effect;
    }
}
