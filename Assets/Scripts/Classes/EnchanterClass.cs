using UnityEngine;

public class EnchanterClass : ClassBase
{
    public override void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("EnchanterClass", 1, "Attack (0.6x base value)");
    }

    public override void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("EnchanterClass", 2, "Next attack leeches 30% of damage done");
    }

    public override void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("EnchanterClass", 3, "Grant target a damage shield that blocks X damage");
    }

    public override void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("EnchanterClass", 4, "Halve the next prompt's length");
    }
}
