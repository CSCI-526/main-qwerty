using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnchanterClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue * 0.6f, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.3f * baseValue;
        if (target.targetingID.Value != playerID)
        {
            gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, mod, 1, "LeechBuff");
        }
        gameManager.addBuffDebuffToListRpc(0, playerID, mod, 1, "LeechBuff");
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        if (target.targetingID.Value != playerID)
        {
            target.ModifyCurrentShieldRpc((int)baseValue);
        }
        gameManager.localPlayer.ModifyCurrentShieldRpc((int)baseValue);
        LogAbility("EnchanterClass", 3, "Grant target a damage shield of x% base value");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.4f * baseValue;
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.0f + mod, 3, "DamageTakenDebuff");
        LogAbility("EnchanterClass", 3, "Debuff: target takes +40% damage (3 hits)");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassDSkill1",
        "ClassDSkill2",
        "ClassDSkill3",
        "ClassDSkill4"
    };

    public override string className => "Enchanter";
    
    public override List<string> instructionText { get; } = new List<string>
    {
        "Each class has 4 abilities. Press 1 to try out the first one.",
        "This attacks enemies. Enter the enemy's <color=yellow>Target Word</color> (Word in yellow):",
        "Enemy targeted, type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This buffs a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This buffs a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This debuffs an enemy. Enter an enemy's <color=yellow>Target Word</color>:",
        "Enemy targeted, type the prompt below.",
        "Now everyone finish off the enemy. Select an ability.",
        "Enter a <color=yellow>Target Word</color>:",
        "Type the prompt below."
    };

    public override List<string[]> targetList { get; } = new List<string[]>
    {
        new string[] { "Enemy", "Projectile" },
        new string[] { "Player" },
        new string[] { "Player" },
        new string[] { "Enemy" },
        new string[] { "Projectile" }
    };

    public override List<string> classDescription { get; } = new List<string>
    {
        "Enchanter",
        "Applying buffs to an ally also applies it to yourself.",
        "A weak attack that does less damage.",
        "Makes the player's next attack leech for 30% of the damage done.",
        "Gives a player a shield that blocks damage.",
        "Makes an enemy take 40% more damage on the next 3 attacks."
    };

    public override List<string> abilityDescription { get; } = new List<string>
    {
        "Attack",
        "Buff",
        "Shield",
        "Debuff"
    };
}
