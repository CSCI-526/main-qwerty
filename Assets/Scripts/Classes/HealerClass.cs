using UnityEngine;

public class HealerClass : ClassBase
{
    public override void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        //check target health
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.2f, 1, "HealBuff");
        gameManager.playerHealRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("HealerClass", 1, "Heal (1.2x base value) — single target");
    }

    public override void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, -0.25f, 1, "HealBuff");
        //target all
        //check target health
        LogAbility("HealerClass", 2, "Group Heal (0.75x base value) — all allies");
    }

    public override void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        //check target health
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.0f, 1, "HealBuff");
        gameManager.playerHealRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("HealerClass", 3, "Big Heal (2x base value)");
    }

    public override void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        //revive
        LogAbility("HealerClass", 4, "Revive — restore a fallen ally with 20% health");
    }
}
