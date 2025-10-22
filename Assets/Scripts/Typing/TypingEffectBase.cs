using UnityEngine;

public abstract class TypingEffectBase : ScriptableObject
{
    [Header("Effect Name")]
    public string effectName;
    [Header("Effect Description")]
    [TextArea]
    protected string effectDescription;

    public virtual string GetEffectDescription()
    {
        return effectDescription;
    }

    // 0 - Standard, 1 - Doubled, -1 - Halved
    public virtual int ApplyPunishmentMod()
    {
        return 0;
    }

    public virtual int ApplyHealMod()
    {
        return 0;
    }

    public virtual int ApplyDamageMod()
    {
        return 0;
    }

    public abstract string ApplyEffectOnPrompt(ref string prompt);

    public abstract void OnEndTyping(ref int errors);
}
