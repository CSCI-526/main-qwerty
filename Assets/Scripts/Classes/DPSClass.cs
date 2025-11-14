using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DPSClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.2f, 1, "DamageBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("DPSClass", 1, "Attack (1.2x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.5f, 1, "DamageBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("DPSClass", 2, "Heavy Attack (1.5x base value) — powerful, deliberate strike");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, 1.0f + baseValue, 1, "DamageBuff");
        gameManager.playerHealRpc(playerID, 0, playerID, (int)(-0.3 * target.maxHealth));
        LogAbility("DPSClass", 3, "Sacrifice 30% max HP: next attack deals 2x damage (stacks with passive)");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, baseValue, 1, "LeechBuff");
        gameManager.playerHealRpc(playerID, 0, playerID, (int)(-0.3 * target.maxHealth));
        LogAbility("DPSClass", 4, "Sacrifice 30% max HP: next attack leeches 100% of its damage");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassBSkill1",
        "ClassBSkill2",
        "ClassBSkill3",
        "ClassBSkill4"
    };

    public override string className => "DPS";
}
