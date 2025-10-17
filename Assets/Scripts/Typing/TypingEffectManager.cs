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
}
