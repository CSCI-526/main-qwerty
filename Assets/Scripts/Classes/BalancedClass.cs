using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BalancedClass : ClassBase
{
    public override void Ability1(ulong playerID, TargetableController target, int baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 0.9f, 1, "DamageBuff");
        gameManager.addBuffDebuffToListRpc(0, playerID, 0.2f, 1, "LeechBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        int delta = Math.Min((int)(-baseValue / Math.Pow(2, mod)), -1);
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
        int delta = Math.Max((int)(baseValue / Math.Pow(2, mod)), 1);
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
}
