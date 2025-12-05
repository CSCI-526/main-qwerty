public class ForceCapitalizeCurseData : TypingEffectBase
{
    private char capitalizedLetter;

    public void Initialize(char capitalizedLetter)
    {
        this.capitalizedLetter = char.ToLower(capitalizedLetter);
    }

    public char GetCapitalizedLetter()
    {
        return this.capitalizedLetter;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        return prompt.Replace(capitalizedLetter, char.ToUpper(capitalizedLetter));
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
            return $"All letter " + char.ToUpper(capitalizedLetter) + " capitalized.";
        }
    }

    public override bool IsCurse()
    {
        return true;
    }

    // Curses of same forced capitalized letters are equal
    public override bool Equals(object obj)
    {
        if (obj is ForceCapitalizeCurseData other)
        {
            return this.capitalizedLetter == other.capitalizedLetter;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return capitalizedLetter.GetHashCode();
    }
}
