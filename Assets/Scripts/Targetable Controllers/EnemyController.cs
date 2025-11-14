using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyController : TargetableController
{
    [Header("Enemy Settings")]
    [SerializeField] protected float attackCooldown = 10;
    protected float attackCd = 0;
    protected bool tutorial = false;
    protected int damage = 20;

    protected List<string> wordList = new List<string>();

    [Header("GameObjects")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected GameObject projectileStartingPoint;

    private void Start()
    {
        attackCd = attackCooldown;
        if(gameManager.gameLoopManager.GetTutorialState())
            attackCd = 0.1f;
    }

    public override void OnNetworkSpawn()
    {
        InitTargeting();
        InitHealth();
        RandomizeTargetWord();
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaxHealthRpc(float multiplier)
    {
        maxHealth = (int)(maxHealth * multiplier);
        UpdateCurrentHealthRpc(maxHealth);
        OnHealthChanged(currentHealth.Value, maxHealth);
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaxHealthAmountRpc(int amount)
    {
        maxHealth = amount;
        UpdateCurrentHealthRpc(maxHealth);
        OnHealthChanged(maxHealth, maxHealth);
    }

    [Rpc(SendTo.Everyone)]
    public void SetAttackCooldownRpc(float multiplier)
    {
        attackCooldown *= multiplier;
    }

    public void SetTutorial(bool flag)
    {
        tutorial = flag;
        if(flag)
        {
            SetMaxHealthAmountRpc(1000);
            damage = 5;
        }
    }

    protected override void Die()
    {
        gameManager.RemoveEnemy(targetingID.Value);
        gameObject.GetComponent<NetworkObject>().Despawn(false);
        Destroy(gameObject);
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        targetWordText.text = newWord.ToString();
    }

    protected override void OnTargetIDChanged(ulong oldID, ulong newID)
    {
        gameManager.RefreshEnemies();
    }

    public float GetAttackCooldown() {  return attackCooldown; }

    private void Update()
    {
        if (!IsOwner) return;
        if (IsDead()) return;

        if (!tutorial || currentHealth.Value != maxHealth)
        {
            if (attackCd <= 0)
            {
                ShootWord(gameManager.GenerateWord());
                attackCd = attackCooldown;
            }
            else
            {
                attackCd -= Time.deltaTime;
            }
        }
    }

    protected virtual void ShootWord(string word)
    {
        PlayerController targetPlayer = gameManager.GetRandomPlayer();

        if(targetPlayer == null) return;

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
        pc.SetDamage(damage);
        pc.targetingID.Value = ++gameManager.projectileTargetingIdCounter;

        gameManager.AddProjectile(new ProjectileNetworkData
        {
            TargetingID = gameManager.projectileTargetingIdCounter
        });

        wordList.Add(word);
    }

    public void RemoveWord(string word)
    {
        wordList.Remove(word);
    }

    public List<string> GetWordList()
    {
        return wordList;
    }

}
