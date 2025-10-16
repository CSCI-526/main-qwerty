using UnityEngine;

public class AutoCorrectBuffData : TypingEffectBase
{
    private int autoCorrectCount;

    public void Initialize(int autoCorrectCount)
    {
        this.autoCorrectCount = autoCorrectCount;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        return prompt;
    }

    public override string GetEffectDescription()
    {
        if (effectDescription != null)
        {
            return effectDescription;
        }
        else
        {
            return "Auto Correct Buff";
        }
    }

    public override void OnEndTyping(ref int errors)
    {
        errors = Mathf.Max(0, errors - autoCorrectCount);
    }
}
