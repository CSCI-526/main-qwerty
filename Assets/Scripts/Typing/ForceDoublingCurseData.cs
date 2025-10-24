using System.Text;

public class ForceDoublingCurseData : TypingEffectBase
{
    private char doubledLetter;
    private bool isCaseSensitive;

    public void Initialize(char doubledLetter, bool isCaseSensitive)
    {
        this.doubledLetter = doubledLetter;
        this.isCaseSensitive = isCaseSensitive;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        var stringBuilder = new StringBuilder();
        foreach (char c in prompt)
        {
            if (isCaseSensitive)
            {
                if (c == doubledLetter)
                {
                    stringBuilder.Append(c);
                }
            }
            else
            {
                if (char.ToLower(c) == char.ToLower(doubledLetter))
                {
                    stringBuilder.Append(c);
                }
            }
            stringBuilder.Append(c);
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

        if (isCaseSensitive)
        {
            return $"Letter {doubledLetter} doubled";
        }
        else
        {
            return $"Letter {char.ToLower(doubledLetter)}/{char.ToUpper(doubledLetter)} doubled";
        }
    }

    // Curses of same doubled letters are equal
    public override bool Equals(object obj)
    {
        if (obj is ForceDoublingCurseData other)
        {
            return this.doubledLetter == other.doubledLetter;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return doubledLetter.GetHashCode();
    }
}
