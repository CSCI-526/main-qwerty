using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BalancedClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 0.9f, 1, "DamageBuff");
        gameManager.addBuffDebuffToListRpc(0, playerID, 0.2f, 1, "LeechBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("BalancedClass", 1, "Quick strike (0.9x base attack)");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.5f * baseValue;
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.0f + mod, 1, "DamageBuff");
        LogAbility("BalancedClass", 2, "Self buff: next attack +50% damage");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.3f * baseValue;
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.0f + mod, 3, "DamageTakenDebuff");
        LogAbility("BalancedClass", 3, "Debuff: target takes +30% damage (2 hits)");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
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
}
