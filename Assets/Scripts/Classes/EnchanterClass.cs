using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnchanterClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        gameManager.addBuffDebuffToListRpc(0, playerID, 0.6f, 1, "DamageBuff");
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
        baseValue = Mathf.Clamp(baseValue * maxDamageValue, 1, int.MaxValue);
        int delta = Math.Min((int)(-baseValue / Math.Pow(modMultipler, mod)), -1);
        gameManager.playerDealDamageRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.3f * baseValue;
        if (target.targetingID.Value != playerID)
        {
            gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, mod, 1, "LeechBuff");
        }
        gameManager.addBuffDebuffToListRpc(0, playerID, mod, 1, "LeechBuff");
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        if (target.targetingID.Value != playerID)
        {
            target.ModifyCurrentShieldRpc((int)baseValue);
        }
        gameManager.localPlayer.ModifyCurrentShieldRpc((int)baseValue);
        LogAbility("EnchanterClass", 3, "Grant target a damage shield of x% base value");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        float mod = 0.4f * baseValue;
        gameManager.addBuffDebuffToListRpc(targetType, target.targetingID.Value, 1.0f + mod, 3, "DamageTakenDebuff");
        LogAbility("EnchanterClass", 3, "Debuff: target takes +40% damage (3 hits)");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassDSkill1",
        "ClassDSkill2",
        "ClassDSkill3",
        "ClassDSkill4"
    };
}
