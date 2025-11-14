using System.Collections.Generic;
using UnityEngine;

public class HealerClass : ClassBase
{
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        //check target health
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.2f, 1, "HealBuff");
        //gameManager.playerHealRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("HealerClass", 1, "Heal (1.2x base value) — single target");
    }

    public override void Ability2(ulong playerID, TargetableController target, int baseValue)
    {
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, -0.25f, 1, "HealBuff");
        //target all
        //check target health
        LogAbility("HealerClass", 2, "Group Heal (0.75x base value) — all allies");
    }

    public override void Ability3(ulong playerID, TargetableController target, int baseValue)
    {
        //check target health
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.0f, 1, "HealBuff");
        //gameManager.playerHealRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("HealerClass", 3, "Big Heal (2x base value)");
    }

    public override void Ability4(ulong playerID, TargetableController target, int baseValue)
    {
        //revive
        LogAbility("HealerClass", 4, "Revive — restore a fallen ally with 20% health");
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
        "This ability heals a player. Enter a player's <color=yellow>Target Word</color> (Word in yellow):",
        "Type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This ability heals all players. Enter a player's <color=yellow>Target Word</color>:",
        "Type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This heals a player for more. Enter an enemy's <color=yellow>Target Word</color>:",
        "Type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This revives a player. Enter a player's <color=yellow>Target Word</color>:",
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

    public override List<string> classDescription { get; } = new List<string>
    {
        "Healer",
        "Healing are 30% stronger on player's with less than 50% health.",
        "Heal a single player.",
        "Heal all players for 75% effectiveness.",
        "Heal a player for double the value.",
        "Revive a player and set their health to 20%."
    };
}
