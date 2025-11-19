using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HealerClass : ClassBase
{
    private float modMultipler = 1.2f;
    public override void Ability1(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        gameManager.addBuffDebuffToListRpc(0, playerID, 1.2f, 1, "HealBuff");
        if (target.currentHealth.Value <= target.maxHealth / 2)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, 1.3f, 1, "HealBuff");
        }
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("HealerClass", 1, "Heal (1.2x base value) � single target");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        List<PlayerController> playerControllers = gameManager.GetAllPlayers();
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        foreach (PlayerController player in playerControllers)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, 0.75f, 1, "HealBuff");
            if (player.currentHealth.Value <= player.maxHealth / 2)
            {
                gameManager.addBuffDebuffToListRpc(0, playerID, 1.3f, 1, "HealBuff");
            }
            gameManager.playerHealRpc(playerID, 0, player.targetingID.Value, delta);
        }
        LogAbility("HealerClass", 2, "Group Heal (0.75x base value) � all allies");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        gameManager.addBuffDebuffToListRpc(0, playerID, 2.0f, 1, "HealBuff");
        if (target.currentHealth.Value <= target.maxHealth / 2)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, 1.3f, 1, "HealBuff");
        }
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("HealerClass", 3, "Big Heal (2x base value)");
    }

    public override void Ability4(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        if (targetType != 0 || !target.IsDead() || baseValue < 0.3) return;

        target.ReviveRpc();
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, (int)(0.2f * target.maxHealth));
        LogAbility("HealerClass", 4, "Revive � restore a fallen ally with 20% health");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassCSkill1",
        "ClassCSkill2",
        "ClassCSkill3",
        "ClassCSkill4"
    };

    public override string className => "Healer";
    
    public override List<string> instructionText { get; } = new List<string>
    {
        "Each class has 4 abilities. Press 1 to try out the first one.",
        "This ability heals a player. Enter a player's <color=yellow>Target Word</color> (Word in yellow):",
        "Type the prompt below.",
        "Let's try the 2nd ability now. Press 2.",
        "This ability heals all players. Enter a player's <color=yellow>Target Word</color>:",
        "Type the prompt below.",
        "Let's try the 3rd ability. Press 3.",
        "This heals a player for more. Enter an enemy's <color=yellow>Target Word</color>:",
        "Type the prompt below.",
        "Let's try the 4th ability. Press 4.",
        "This revives a player. Enter a player's <color=yellow>Target Word</color>:",
        "Player targeted, type the prompt below.",
        "Now everyone finish off the enemy. Select an ability.",
    };

    public override List<string> promptText { get; } = new List<string>
    {
        "Press Tab to view your abilities and stats.",
        "Typos will inflict damage to yourself.",
        "Attack projectiles to destroy them.",
        "Defeat the enemy to progress."
    };

    public override List <string> targetList { get; } = new List<string>
    {
        "Player",
        "Player",
        "Player",
        "Player",
        "Projectile"
    };

    public override List<string> classDescription { get; } = new List<string>
    {
        "Healer",
        "Healing are 30% stronger on player's with less than 50% health.",
        "Heal a single player.",
        "Heal all players for 75% effectiveness.",
        "Heal a player for double the value.",
        "Revive a player and set their health to 20%."
    };

    public override List<string> abilityDescription { get; } = new List<string>
    {
        "Heal",
        "Heal",
        "Heal",
        "Revive"
    };
}
