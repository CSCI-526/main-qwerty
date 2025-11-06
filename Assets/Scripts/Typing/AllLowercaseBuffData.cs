using System.Text;

public class AllLowercaseBuffData : TypingEffectBase
{
    public void Initialize()
    {
        return;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        var stringBuilder = new StringBuilder();
        foreach (char c in prompt)
        {
            stringBuilder.Append(char.ToLower(c));
        }
        return stringBuilder.ToString();
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
            return $"No uppercase letters";
        }
    }

    public override bool IsCurse()
    {
        return false;
    }

    // Curses of same forced capitalized letters are equal
    public override bool Equals(object obj)
    {
        if (obj is AllLowercaseBuffData other)
        {
            return true;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return "AllLowercaseBuffData".GetHashCode();
    }
}
