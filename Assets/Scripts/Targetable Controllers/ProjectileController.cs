using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController : TargetableController
{
    [Header("Projectile Settings")]
    [SerializeField] private int wordSpeed = 5;
    [SerializeField] private int damage = 50;

    private string word = "";

    private TargetableController spawner;
    private TargetableController target;

    public override void OnNetworkSpawn()
    {
        InitHealth();
    }

    void Update()
    {
        if(!IsOwner) return;

        MoveTowardsTarget();
    }

    protected override void Die()
    {
        gameManager.RemoveProjectile(targetingID.Value);
        gameObject.GetComponent<NetworkObject>().Despawn(false);
        Destroy(gameObject);
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        targetWordText.text = newWord.ToString();
    }

    private void MoveTowardsTarget()
    {
        if (target == null || target.IsDead())
        {
            Die();
        }
        else
        {
            int mod = 0;
            if (target.targetingID.Value == gameManager.localPlayer.targetingID.Value)
            {
                mod = gameManager.typingEffectManager.ApplyEffectOnMod()[3];
            }
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * (mod == 0 ? wordSpeed : mod == 1 ? wordSpeed * 2 : wordSpeed / 2) * Time.deltaTime);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateTextEveryoneRpc(FixedString128Bytes newWord) {
        word = newWord.ToString();
        GetComponent<TMP_Text>().text = word; 
    }
    public void SetSpawner(TargetableController obj) { spawner = obj; }
    public void SetTarget(TargetableController obj) { target = obj; }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (!IsOwner) return;

        if (other.gameObject == target.gameObject)
        {
            string word = gameObject.GetComponent<TMP_Text>().text;
            target.GetComponent<PlayerController>().ModifyCurrentHealth(-damage);
            spawner.GetComponent<EnemyController>().RemoveWord(word);
            Die();
        }
    }
}
