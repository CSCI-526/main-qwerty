using UnityEngine;

public class BalancedClass : ClassBase
{
    public override void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.9f, 1, "DamageBuff");
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.2f, 1, "LeechBuff");
        gameManager.playerDealDamageRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("BalancedClass", 1, "Quick strike (0.9x base attack)");
    }

    public override void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.5f, 1, "DamageBuff");
        LogAbility("BalancedClass", 2, "Self buff: next attack +50% damage");
    }

    public override void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 1.3f, 3, "DamageTakenDebuff");
        LogAbility("BalancedClass", 3, "Debuff: target takes +30% damage (3 hits)");
    }

    public override void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        gameManager.playerHealRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("BalancedClass", 4, "Heal ally for 1.0x of base value");
    }
}
