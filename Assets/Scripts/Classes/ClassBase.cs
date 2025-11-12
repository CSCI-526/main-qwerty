using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

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
    public abstract void Ability1(ulong playerID, TargetableController target, int baseDamage);
    public abstract void Ability2(ulong playerID, TargetableController target, int baseDamage);
    public abstract void Ability3(ulong playerID, TargetableController target, int baseDamage);
    public abstract void Ability4(ulong playerID, TargetableController target, int baseDamage);

    public abstract List<string> promptFileNames { get; }

    public abstract List<string> instructionText { get; }

    public abstract List<string> promptText { get; }

    // Optional: you can include shared utility methods here
    protected void LogAbility(string className, int abilityNumber, string description)
    {
        Debug.Log($"{className} used Ability {abilityNumber}: {description}");
    }

    protected ulong DetermineTargetType(TargetableController target)
    {
        if (target is PlayerController)
            return 0; // Player
        else if (target is EnemyController)
            return 1; // Enemy
        else if (target is ProjectileController)
            return 2; // Projectile
        else
            return 3; // Unknown
    }
}
