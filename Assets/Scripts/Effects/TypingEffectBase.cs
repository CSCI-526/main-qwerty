using UnityEngine;

public abstract class TypingEffectBase : ScriptableObject
{
    [Header("Effect Name")]
    public string effectName;
    [Header("Effect Description")]
    [TextArea]
    public string effectDescription;

    public abstract string OnInputChanged(ref string currentText, ref string prompt);

    public abstract void OnEndTyping(ref int errors);
}
