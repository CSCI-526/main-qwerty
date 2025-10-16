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

    public abstract string ApplyEffectOnPrompt(ref string prompt);

    public abstract void OnEndTyping(ref int errors);
}
