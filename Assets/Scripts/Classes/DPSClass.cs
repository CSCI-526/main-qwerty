using System.Collections.Generic;
using UnityEngine;

public class DPSClass : ClassBase
{
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.2f, 1, "DamageBuff");
        //gameManager.playerDealDamageRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("DPSClass", 1, "Attack (1.2x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, int baseValue)
    {
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.5f, 1, "DamageBuff");
        //gameManager.playerDealDamageRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("DPSClass", 2, "Heavy Attack (1.5x base value) — powerful, deliberate strike");
    }

    public override void Ability3(ulong playerID, TargetableController target, int baseValue)
    {
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 2.0f, 1, "DamageBuff");
        //gameManager.playerHealRpc(playerID, targetType, targetingID, -30);
        LogAbility("DPSClass", 3, "Sacrifice 30% max HP: next attack deals 2x damage (stacks with passive)");
    }

    public override void Ability4(ulong playerID, TargetableController target, int baseValue)
    {
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.0f, 1, "HealBuff");
        //gameManager.playerHealRpc(playerID, targetType, targetingID, -30);
        LogAbility("DPSClass", 4, "Sacrifice 30% max HP: next attack leeches 100% of its damage");
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
        "This ability attacks projectiles and enemies. Enter the enemy's <color=yellow>Target Word</color> (Word in yellow):",
        "Enemy targeted, type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This ability buffs allies. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This debuffs an enemy. Enter an enemy's <color=yellow>Target Word</color>:",
        "Enemy targeted, type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This heals a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Now everyone finish off the enemy. Select an ability.",
        "Enter a <color=yellow>Target Word</color>:",
        "Type the prompt below."
    };

    public override List<string> promptText { get; } = new List<string>
    {
        "This damages the enemy and leeches 30% health.",
        "This increases an ally's attack by 50%",
        "This decreases an enemy's attack by 30%:",
        "Speed & accuracy determines an ability's effectiveness.",
        "Press Tab if you forget what the abilities do."
    };
}
