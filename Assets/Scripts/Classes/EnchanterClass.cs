using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnchanterClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, 0.6f, 1, "DamageBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        if (target.targetingID.Value != playerID){
            gameManager.addBuffDebuffToListRpc(0, playerID, 0.3f, 1, "LeechBuff");
        }
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 0.3f, 1, "LeechBuff");
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        if (target.targetingID.Value != playerID)
        {
            gameManager.playerHealRpc(playerID, 0, playerID, delta);
        }
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("EnchanterClass", 3, "Grant target a damage shield of x% base value");
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
}
