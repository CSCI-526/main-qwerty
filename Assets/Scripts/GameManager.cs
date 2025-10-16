using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Players and Enemies")]
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>();
    [SerializeField] private List<ProjectileController> projectiles = new List<ProjectileController>();

    [Header("GameObjects")]
    [SerializeField] private GameObject projectileParent;

    #region Players

    public void AddPlayer(PlayerController player)
    {
        players.Add(player);
    }

    public void RemovePlayer(PlayerController player)
    {
        players.Remove(player);
    }

    public PlayerController GetRandomPlayer()
    {
        if (players.Count == 0) return null;
        int randomIndex = Random.Range(0, players.Count);
        return players[randomIndex];
    }

    #endregion

    #region Enemies

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    #endregion

    #region Projectiles

    public void AddProjectile(ProjectileController projectile)
    {
        projectiles.Add(projectile);
    }

    public void RemoveProjectile(ProjectileController projectile)
    {
        projectiles.Remove(projectile);
    }

    public GameObject GetProjectileParent() { return projectileParent; }

    #endregion
}
