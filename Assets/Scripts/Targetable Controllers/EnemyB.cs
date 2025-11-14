using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyB : EnemyController
{
    // This enemy will spawn projectiles that need to be typed in reversed order
    // color - magenta, special code - 1

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
        pc.UpdateTextEveryoneRpc(new FixedString128Bytes(word), special: 1);
        pc.SetTargetWord(word);
        pc.SetSpawner(this);
        pc.SetTarget(targetPlayer);
        pc.targetingID.Value = ++gameManager.projectileTargetingIdCounter;

        gameManager.AddProjectile(new ProjectileNetworkData
        {
            TargetingID = pc.targetingID.Value
        });

        wordList.Add(word);
    }

}
