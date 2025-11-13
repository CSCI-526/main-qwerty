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

    public override List<string> classDescription { get; } = new List<string>
    {
        "Balanced",
        "Attacks leech 20% of damage dealt.",
        "A quick attack that does a little less damage and generates a short prompt.",
        "Buff yourself or an ally to increase the damage of the next attack by 50%",
        "Debuff an enemy to make them take 30% more damage on the next 3 attacks.",
        "Heal yourself or an ally."
    };
}
