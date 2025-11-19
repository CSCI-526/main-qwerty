using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyC : EnemyController
{
    // This enemy will spawn projectiles that can only be seen by the target player
    // color - red, special code - 2

    protected override void ShootWord(string word)
    {
        PlayerController targetPlayer = gameManager.GetRandomPlayer();

        if (targetPlayer == null) return;

        GameObject projectile = Instantiate(projectilePrefab, projectileStartingPoint.transform.position, Quaternion.identity);
        projectile.GetComponent<NetworkObject>().Spawn(true);

        projectile.transform.SetParent(gameManager.GetProjectileParent().transform);
        projectile.transform.rotation = projectileStartingPoint.transform.rotation;
        projectile.transform.localScale = Vector3.one;

        ProjectileController pc = projectile.GetComponent<ProjectileController>();
        // Mark the word as ??????
        pc.UpdateTextEveryoneRpc(new FixedString128Bytes(word), special: 2);
        pc.SetTargetWord(word);
        pc.SetSpawner(this);
        pc.SetTarget(targetPlayer);

        // Update the text to the target
        pc.UpdateTextClientRpc(new FixedString128Bytes(word), special: 2, rpcParams: RpcTarget.Single(targetPlayer.GetPlayerID(), RpcTargetUse.Temp));
        pc.targetingID.Value = ++gameManager.projectileTargetingIdCounter;

        gameManager.AddProjectile(new ProjectileNetworkData
        {
            TargetingID = pc.targetingID.Value
        });

        wordList.Add(word);
    }
}
