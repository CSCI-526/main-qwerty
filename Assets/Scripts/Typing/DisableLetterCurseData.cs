using System.Text;

public class DisableLetterCurseData : TypingEffectBase
{
    private char disabledLetter;
    private bool isCaseSensitive;

    public void Initialize(char disabledLetter, bool isCaseSensitive)
    {
        this.disabledLetter = disabledLetter;
        this.isCaseSensitive = isCaseSensitive;
    }

    public override string ApplyEffectOnPrompt(ref string prompt)
    {
        if (isCaseSensitive)
        {
            return prompt.Replace(disabledLetter.ToString(), "");
        }
        else
        {
            var stringBuilder = new StringBuilder();
            foreach (char c in prompt)
            {
                if (char.ToLower(c) != char.ToLower(disabledLetter))
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString();
        }
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
            return $"Letter {disabledLetter} disabled";
        }
        else
        {
            return $"Letter {char.ToLower(disabledLetter)}/{char.ToUpper(disabledLetter)} disabled";
        }
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
