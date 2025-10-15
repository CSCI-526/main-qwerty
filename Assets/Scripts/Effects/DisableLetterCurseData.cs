public class DisableLetterCurseData : TypingEffectBase
{
    private char disabledLetter;

    public void Initialize(char disabledLetter)
    {
        this.disabledLetter = disabledLetter;
    }

    public override string OnInputChanged(ref string currentText, ref string prompt)
    {
        int len = currentText.Length;
        if (len > 0 && len <= prompt.Length && prompt[len - 1] == disabledLetter)
        {
            return prompt[..(len - 1)] + " " + prompt[len..];
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
        if (obj is DisableLetterCurseData other)
        {
            return this.disabledLetter == other.disabledLetter;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return disabledLetter.GetHashCode();
    }
}
