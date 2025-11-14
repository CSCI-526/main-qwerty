using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyD : EnemyController
{
    // The enemy may shoot words to multiple players at the same time
    // No special code needed

    protected override void ShootWord(string word)
    {
        List<PlayerController> targetPlayers = gameManager.GetRandomPlayers();

        if (targetPlayers == null) return;

        foreach (PlayerController targetPlayer in targetPlayers)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileStartingPoint.transform.position, Quaternion.identity);
            projectile.GetComponent<NetworkObject>().Spawn(true);

            projectile.transform.SetParent(gameManager.GetProjectileParent().transform);
            projectile.transform.rotation = projectileStartingPoint.transform.rotation;
            projectile.transform.localScale = Vector3.one;

            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            pc.UpdateTextEveryoneRpc(new FixedString128Bytes(word));
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
}
