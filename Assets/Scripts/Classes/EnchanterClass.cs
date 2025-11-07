using UnityEngine;

public class EnchanterClass : ClassBase
{
    public override void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        /*if (targetID != self){
            gameManager.addBuffDebuffToListRpc(targetType, self, 0.6f, 1, "DamageBuff");
            gameManager.playerDealDamageRpc(self, targetType, self, baseValue);
         }*/
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.6f, 1, "DamageBuff");
        gameManager.playerDealDamageRpc(playerID, targetType, targetingID, baseValue);
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        /*if (targetID != self){
            gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.3f, 1, "LeechBuff");
         }*/
        gameManager.addBuffDebuffToListRpc(targetType, targetingID, 0.3f, 1, "LeechBuff");
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        /*if (targetID != self){
            //shield
         }*/
        //shield
        LogAbility("EnchanterClass", 3, "Grant target a damage shield that blocks X damage");
    }

    public override void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        /*if (targetID != self){
            //change prompt length
         }*/
        //change prompt length
        LogAbility("EnchanterClass", 4, "Halve the next prompt's length");
    }
}
