using UnityEngine;
using Unity.VisualScripting;

public abstract class ClassBase : MonoBehaviour
{
    [DoNotSerialize]
    public GameManager gameManager => FindFirstObjectByType<GameManager>();

    // Common properties for all classes (optional)
    [Header("Base Stats")]
    public float baseAttackValue = 100f;
    public float baseHealValue = 100f;

    // --- Abstract Abilities ---
    // Each subclass must override these
    public abstract void Ability1(ulong playerID, ulong targetType, ulong targetingID, int baseDamage);
    public abstract void Ability2(ulong playerID, ulong targetType, ulong targetingID, int baseDamage);
    public abstract void Ability3(ulong playerID, ulong targetType, ulong targetingID, int baseDamage);
    public abstract void Ability4(ulong playerID, ulong targetType, ulong targetingID, int baseDamage);

    // Optional: you can include shared utility methods here
    protected void LogAbility(string className, int abilityNumber, string description)
    {
        Debug.Log($"{className} used Ability {abilityNumber}: {description}");
    }
}
