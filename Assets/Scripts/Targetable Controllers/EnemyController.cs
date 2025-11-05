using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyController : TargetableController
{
    [Header("Enemy Settings")]
    [SerializeField] private float attackCooldown = 10;
    private float attackCd = 0;

    private List<string> wordList = new List<string>();

    [Header("GameObjects")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileStartingPoint;

    private void Start()
    {
        attackCd = attackCooldown;
    }

    public override void OnNetworkSpawn()
    {
        InitTargeting();
        RandomizeTargetWord();
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaxHealthRpc(float multiplier)
    {
        maxHealth = (int)(maxHealth * multiplier);
        UpdateCurrentHealthRpc(maxHealth);
        OnHealthChanged(currentHealth.Value, maxHealth);
    }

    public void SetAttackCooldown(float multiplier)
    {
        attackCooldown *= multiplier;
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

    private void Update()
    {
        if (!IsOwner) return;
        if (IsDead()) return;

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

    private void ShootWord(string word)
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
        pc.targetingID.Value = ++gameManager.projectileTargetingIdCounter;

        gameManager.AddProjectile(new ProjectileNetworkData
        {
            TargetingID = pc.targetingID.Value
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
