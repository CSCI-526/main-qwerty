using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TypingEffectManager : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText; // list of current curses & buffs

    private List<TypingEffectBase> activeTypingEffects = new(); // currently active curses & buffs

    private void Start()
    {
        UpdateEffectText();
    }

    private void OnEffectChange()
    {
        UpdateEffectText();
    }

    private void UpdateEffectText()
    {
        if (effectText != null)
        {
            string desc = string.Join(", ", activeTypingEffects.Select(e => e.GetEffectDescription()));
            effectText.text = desc;
        }
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
        int[] output = { 0, 0, 0 };
        foreach (var typingEffect in activeTypingEffects)
        {
            if (typingEffect.ApplyPunishmentMod() != 0)
            {
                output[0] = typingEffect.ApplyPunishmentMod();
            }

            if (typingEffect.ApplyHealMod() != 0)
            {
                output[1] = typingEffect.ApplyHealMod();
            }

            if (typingEffect.ApplyDamageMod() != 0)
            {
                output[2] = typingEffect.ApplyDamageMod();
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
    /// Interface for adding effect.
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
    /// Shorthand for adding DisableLetter Curse.
    /// </summary>
    /// <param name="letter">The letter to be disabled (case sensitive or not).</param>
    public void DisableLetter(char letter, bool isCaseSensitive = false)
    {
        var effect = ScriptableObject.CreateInstance<DisableLetterCurseData>();
        effect.Initialize(letter, isCaseSensitive);
        AddTypingEffect(effect);
    }

    /// <summary>
    /// Shorthand for adding ForceCapitalize Curse.
    /// </summary>
    /// <param name="letter">The letter that is forced capitalized.</param>
    public void ForceCapitalize(char letter)
    {
        var effect = ScriptableObject.CreateInstance<ForceCapitalizeCurseData>();
        effect.Initialize(letter);
        AddTypingEffect(effect);
    }

    /// <summary>
    /// Shorthand for adding AutoCorrect Buff.
    /// </summary>
    /// <param name="count">The autocorrect quota to be added.</param>
    public void AddAutoCorrect(int count)
    {
        var effect = ScriptableObject.CreateInstance<AutoCorrectBuffData>();
        effect.Initialize(count);
        AddTypingEffect(effect);
    }


    /// <summary>
    /// Shorthand for adding Punishment Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - take double damage) or buff (-1 - take half damage).</param>
    public void PunishmentMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(curse, 0, 0);
        AddTypingEffect(effect);
    }

    /// <summary>
    /// Shorthand for adding Punishment Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - damage halved) or buff (-1 - damage doubled).</param>
    public void DamageMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(0, curse, 0);
        AddTypingEffect(effect);
    }

    /// <summary>
    /// Shorthand for adding Punishment Buff/Curse.
    /// </summary>
    /// <param name="curse">It is a curse (1 - heal halved) or buff (-1 - heal doubled).</param>
    public void HealMod(int curse)
    {
        var effect = ScriptableObject.CreateInstance<ModBuffCurse>();
        effect.Initialize(0, curse, 0);
        AddTypingEffect(effect);
    }
}
