using System.Collections.Generic;
using UnityEngine;

public class EnchanterClass : ClassBase
{
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        /*if (targetID != self){
            gameManager.addBuffDebuffToListRpc(targetType, self, 0.6f, 1, "DamageBuff");
            gameManager.playerDealDamageRpc(self, targetType, self, baseValue);
         }*/
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.6f, 1, "DamageBuff");
        //gameManager.playerDealDamageRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, int baseValue)
    {
        /*if (targetID != self){
            gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.3f, 1, "LeechBuff");
         }*/
        //gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.3f, 1, "LeechBuff");
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, TargetableController target, int baseValue)
    {
        /*if (targetID != self){
            //shield
         }*/
        //shield
        LogAbility("EnchanterClass", 3, "Grant target a damage shield that blocks X damage");
    }

    public override void Ability4(ulong playerID, TargetableController target, int baseValue)
    {
        /*if (targetID != self){
            //change prompt length
         }*/
        //change prompt length
        LogAbility("EnchanterClass", 4, "Halve the next prompt's length");
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

    public override List<string> promptText { get; } = new List<string>
    {
        "Press Tab to view your abilities and stats.",
        "Typos will inflict damage to yourself.",
        "Attack projectiles to destroy them.",
        "Defeat the enemy to progress."
    };

    public override List<string> classDescription { get; } = new List<string>
    {
        "Enchanter",
        "Applying buffs to an ally also applies it to yourself.",
        "A weak attack.",
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
