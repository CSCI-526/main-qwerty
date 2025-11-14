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
        if (target.currentHealth.Value <= target.maxHealth / 2)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, 0.3f, 1, "HealBuff");
        }
        gameManager.playerHealRpc(playerID, targetType, target.targetingID.Value, delta);
        LogAbility("HealerClass", 1, "Heal (1.2x base value) — single target");
    }

    public override void Ability2(ulong playerID, TargetableController target, float baseValue)
    {
        List<PlayerController> playerControllers = gameManager.GetAllPlayers();
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        foreach (PlayerController player in playerControllers)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, -0.25f, 1, "HealBuff");
            if (player.currentHealth.Value <= player.maxHealth / 2)
            {
                gameManager.addBuffDebuffToListRpc(0, playerID, 0.3f, 1, "HealBuff");
            }
            gameManager.playerHealRpc(playerID, 1, player.targetingID.Value, delta);
        }
        LogAbility("HealerClass", 2, "Group Heal (0.75x base value) — all allies");
    }

    public override void Ability3(ulong playerID, TargetableController target, float baseValue)
    {
        ulong targetType = DetermineTargetType(target);
        int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
        baseValue = Mathf.Clamp(baseValue * maxHealValue, 1, int.MaxValue);
        int delta = Math.Max((int)(baseValue / Math.Pow(modMultipler, mod)), 1);
        if (target.currentHealth.Value <= target.maxHealth / 2)
        {
            gameManager.addBuffDebuffToListRpc(0, playerID, 1.0f, 1, "HealBuff");
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
        LogAbility("HealerClass", 4, "Revive — restore a fallen ally with 20% health");
    }

    public override List<string> promptFileNames { get; } = new List<string>
    {
        "ClassCSkill1",
        "ClassCSkill2",
        "ClassCSkill3",
        "ClassCSkill4"
    };
}
