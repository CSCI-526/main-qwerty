using UnityEngine;

public class AutoCorrectBuffData : TypingEffectBase
{
    private int autoCorrectCount;

    public void Initialize(int autoCorrectCount)
    {
        this.autoCorrectCount = autoCorrectCount;
    }

    public override string OnInputChanged(ref string currentText, ref string prompt)
    {
        return prompt;
    }

    public override void OnEndTyping(ref int errors)
    {
        errors = Mathf.Max(0, errors - autoCorrectCount);
    }
}
