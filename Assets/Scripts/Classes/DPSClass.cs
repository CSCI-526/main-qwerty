using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPSClass : ClassBase
{
    private float modMultipler = 1.2f;
    private int numProjectilesKilled = 3;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue * 1.2f, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        StartCoroutine(CheckPassiveStacks(playerID, target));
        LogAbility("DPSClass", 1, "Attack (1.2x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue * 1.5f, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        StartCoroutine(CheckPassiveStacks(playerID, target));
        LogAbility("DPSClass", 2, "Heavy Attack (1.5x base value) � powerful, deliberate strike");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, 1.0f + baseValue, 1, "DamageBuff");
        gameManager.localPlayer.ModifyCurrentHealth((int)(-0.3 * gameManager.localPlayer.maxHealth));
        gameManager.damageManager.applyHealthChange(gameManager.localPlayer, (int)-0.3 * gameManager.localPlayer.maxHealth);
        LogAbility("DPSClass", 3, "Sacrifice 30% max HP: next attack deals 2x damage (stacks with passive)");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, baseValue, 1, "LeechBuff");
        gameManager.localPlayer.ModifyCurrentHealth((int)(-0.3 * gameManager.localPlayer.maxHealth));
        gameManager.damageManager.applyHealthChange(gameManager.localPlayer, (int)-0.3 * gameManager.localPlayer.maxHealth);
        LogAbility("DPSClass", 4, "Sacrifice 30% max HP: next attack leeches 100% of its damage");
    }

    private IEnumerator CheckPassiveStacks(ulong playerID, TargetableController target)
    {
        yield return new WaitForSeconds(1f);

        if (DetermineTargetType(target) == 2)
        {
            numProjectilesKilled--;
            if (numProjectilesKilled <= 0)
            {
                gameManager.addBuffDebuffToListRpc(0, playerID, 2.0f, 1, "DamageBuff");
                numProjectilesKilled = 3;
            }
        }
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassBSkill1",
        "ClassBSkill2",
        "ClassBSkill3",
        "ClassBSkill4"
    };

    public override string className => "DPS";
    
    public override List<string> instructionText { get; } = new List<string>
    {
        "Each class has 4 abilities. Press 1 to try out the first one.",
        "This damages an enemy. Enter the enemy's <color=yellow>Target Word</color> (Word in yellow):",
        "Enemy targeted, type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This is a stronger attack. Enter an enemy's <color=yellow>Target Word</color>:",
        "Enemy targeted, type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This buffs yourself. Enter your <color=yellow>Target Word</color>:",
        "Enemy targeted, type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This buffs yourself. Enter your <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Destroy enemy projectiles with an attack (press 1):",
        "Enter the projectile's <color=yellow>Target Word</color>:",
        "Now finish off the enemy. Select any ability."
    };

    public override List<string[]> targetList { get; } = new List<string[]>
    {
        new string[] { "Enemy", "Projectile" },
        new string[] { "Enemy", "Projectile" },
        new string[] { "Player" },
        new string[] { "Player" },
        new string[] { "Projectile" }
    };

    public override List<string> classDescription { get; } = new List<string>
    {
        "DPS",
        "After destroying 3 projectiles, your next attack does double damage",
        "A basic attack.",
        "A stronger attack that requires a longer prompt.",
        "Sacrifice 30% of your max health, making your next attack do double damage.",
        "Sacrifice 30% of your max health, making your next attack leech for its damage dealt."
    };

    public override List<string> abilityDescription { get; } = new List<string>
    {
        "Attack",
        "Attack",
        "Buff",
        "Buff"
    };
}
