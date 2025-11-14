using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.Table;

public class BalancedClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 0.9f, 1, "DamageBuff");
        gameManager.addBuffDebuffToListRpc(0, playerID, 0.2f, 1, "LeechBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("BalancedClass", 1, "Quick strike (0.9x base attack)");
    }

    public override void Ability2(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.5f, 1, "DamageBuff");
        LogAbility("BalancedClass", 2, "Self buff: next attack +50% damage");
    }

    public override void Ability3(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.3f, 3, "DamageTakenDebuff");
        LogAbility("BalancedClass", 3, "Debuff: target takes +30% damage (3 hits)");
    }

    public override void Ability4(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("BalancedClass", 4, "Heal ally for 1.0x of base value");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassASkill1",
        "ClassASkill2",
        "ClassASkill3",
        "ClassASkill4"
    };

    public override List<string> instructionText { get; } = new List<string>
    {
        "Each class has 4 abilities. Press 1 to try out the first one.",
        "This ability damages enemies. Enter the enemy's <color=yellow>Target Word</color> (Word in yellow):",
        "Enemy targeted, type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This ability buffs a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This debuffs an enemy. Enter an enemy's <color=yellow>Target Word</color>:",
        "Enemy targeted, type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This heals a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Destroy enemy projectiles with an attack (press 1):",
        "Enter the projectile's <color=yellow>Target Word</color>:",
        "Now finish off the enemy. Select any ability."
    };

    public override List<string> promptText { get; } = new List<string>
    {
        "Press Tab to view your abilities and stats.",
        "Typos will inflict damage to yourself.",
        "Attack projectiles to destroy them.",
        "Defeat the enemy to progress."
    };

    public override List<string> classDescription { get; } = new List<string>
    {
        "Balanced",
        "Attacks leech 20% of damage dealt.",
        "A quick attack.",
        "Buff yourself or an ally to increase the damage of the next attack by 50%",
        "Debuff an enemy to make them take 30% more damage on the next 2 attacks.",
        "Heal a Player."
    };

    public override List<string> abilityDescription { get; } = new List<string>
    {
        "Attack",
        "Buff",
        "Debuff",
        "Heal"
    };
}
