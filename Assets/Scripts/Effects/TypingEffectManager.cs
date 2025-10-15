using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TypingEffectManager : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText; // list of current curses & buffs

    [SerializeField] private List<TypingEffectBase> activeTypingEffects = new(); // currently active curses & buffs

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
            string desc = string.Join(", ", activeTypingEffects.Select(e => e.effectDescription));
            effectText.text = desc;
        }
    }

    public string OnInputChanged(ref string currentText, ref string prompt)
    {
        string newPrompt = prompt;
        foreach (var typingEffect in activeTypingEffects)
        {
            newPrompt = typingEffect.OnInputChanged(ref currentText, ref newPrompt);
        }
        return newPrompt;
    }

    public void OnEndTyping(ref int errors)
    {
        foreach (var typingEffect in activeTypingEffects)
        {
            typingEffect.OnEndTyping(ref errors);
        }
    }

    // interface for adding curses & buffs
    public void AddTypingEffect(TypingEffectBase typingEffect)
    {
        if (!activeTypingEffects.Contains(typingEffect))
        {
            activeTypingEffects.Add(typingEffect);
            OnEffectChange();
        }
    }

    // interface for removing curses (mainly) & (some temporary) buffs 
    public void RemoveTypingEffect(TypingEffectBase typingEffect)
    {
        if (activeTypingEffects.Contains(typingEffect))
        {
            activeTypingEffects.Remove(typingEffect);
            OnEffectChange();
        }
    }

    // shortcuts for adding certain curses & buffs
    public void DisableLetter(char letter)
    {
        var effect = ScriptableObject.CreateInstance<DisableLetterCurseData>();
        effect.Initialize(letter);
        AddTypingEffect(effect);
    }
    public void ForceCapitalize(char letter)
    {
        var effect = ScriptableObject.CreateInstance<ForceCapitalizeCurseData>();
        effect.Initialize(letter);
        AddTypingEffect(effect);
    }
    public void AddAutoCorrect(int count)
    {
        var effect = ScriptableObject.CreateInstance<AutoCorrectBuffData>();
        effect.Initialize(count);
        AddTypingEffect(effect);
    }
}
