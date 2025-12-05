using System.Text.RegularExpressions;

public class NoPunctuationBuffData : TypingEffectBase
{
    public void Initialize()
    {
        return;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        return Regex.Replace(prompt, @"[^A-Za-z\s]", "");
    }

    public override void OnEndTyping(ref int errors)
    {
        return;
    }

    public override string GetEffectDescription()
    {
        if (effectDescription != null)
        {
            return effectDescription;
        }
        else
        {
            return $"No Punctuation";
        }
    }

    public override bool IsCurse()
    {
        return false;
    }

    // Multiple instances are treated as equal
    public override bool Equals(object obj)
    {
        if (obj is NoPunctuationBuffData other)
        {
            return true;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return "NoPunctuationBuffData".GetHashCode();
    }
}
