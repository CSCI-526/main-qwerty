using UnityEngine;

public class DPSClass : ClassBase
{
    public override void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("DPSClass", 1, "Attack (1.2x base value)");
    }

    public override void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("DPSClass", 2, "Heavy Attack (1.5x base value) — powerful, deliberate strike");
    }

    public override void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("DPSClass", 3, "Sacrifice 30% max HP: next attack deals 2x damage (stacks with passive)");
    }

    public override void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseValue)
    {
        LogAbility("DPSClass", 4, "Sacrifice 30% max HP: next attack leeches 100% of its damage");
    }
}
