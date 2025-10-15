public class ForceCapitalizeCurseData : TypingEffectBase
{
    private char capitalizedLetter;

    public void Initialize(char capitalizedLetter)
    {
        this.capitalizedLetter = char.ToLower(capitalizedLetter);
    }

    public override string OnInputChanged(ref string currentText, ref string prompt)
    {
        int len = currentText.Length;
        if (len > 0 && len <= prompt.Length && char.ToLower(prompt[len - 1]) == capitalizedLetter)
        {
            return prompt[..(len - 1)] + char.ToUpper(capitalizedLetter) + prompt[len..];
        }
        else
        {
            return prompt;
        }
    }

    public override void OnEndTyping(ref int errors)
    {
        return;
    }

    // Curses of same disabled letters are equal
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
