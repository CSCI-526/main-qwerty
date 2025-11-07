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
}
