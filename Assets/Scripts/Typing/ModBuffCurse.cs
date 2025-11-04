using Unity.VisualScripting;
using UnityEngine;

public class ModBuffCurse : TypingEffectBase
{
    private int punishmentCurse;
    private int healCurse;
    private int damageCurse;
    private int bulletSpeedCurse;

    public void Initialize(int punishmentCurse, int healCurse, int damageCurse, int bulletSpeedCurse)
    {
        this.punishmentCurse = punishmentCurse;
        this.healCurse = healCurse;
        this.damageCurse = damageCurse;
        this.bulletSpeedCurse = bulletSpeedCurse;
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
            string output = "";
            if (this.punishmentCurse != 0)
            {
                output += "Punishment " + (this.punishmentCurse == 1 ? "doubled" : "halved");
            }
            if (this.healCurse != 0)
            {
                output += "Healing " + (this.healCurse == -1 ? "doubled" : "halved");
            }
            if (this.damageCurse != 0)
            {
                output += "Damage " + (this.damageCurse == -1 ? "doubled" : "halved");
            }
            if (this.bulletSpeedCurse != 0)
            {
                output += "Bullet Speed " + (this.bulletSpeedCurse == 1 ? "doubled" : "halved");
            }
            return output;
        }
    }

    public override int ApplyPunishmentMod()
    {
        return this.punishmentCurse;
    }
    public override int ApplyHealMod()
    {
        return this.healCurse;
    }
    public override int ApplyDamageMod()
    {
        return this.damageCurse;
    }
    public override int ApplyBulletSpeedMod()
    {
        return this.bulletSpeedCurse;
    }

    public override void OnEndTyping(ref int errors)
    {
        return;
    }
}
