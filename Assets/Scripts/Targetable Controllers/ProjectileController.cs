using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController : TargetableController
{
    [Header("Projectile Settings")]
    [SerializeField] private int wordSpeed = 5;
    [SerializeField] private int wordSpeedMin = 1;
    [SerializeField] private float modMultiplier = 1.2f;
    [SerializeField] private int damage = 50;
    [SerializeField] public GameObject deathEffect;

    private string word = "";

    private TargetableController spawner;
    private TargetableController target;

    public override void OnNetworkSpawn()
    {
        InitHealth();
        InitTargeting();
    }

    void Update()
    {
        if(!IsOwner) return;

        MoveTowardsTarget();
    }

    protected override void Die()
    {
        ShowDeathRpc();
        gameManager.RemoveProjectile(targetingID.Value);
        StartCoroutine(DestroyAfterWait(0.25f));
    }

    IEnumerator DestroyAfterWait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameObject.GetComponent<NetworkObject>().Despawn(false);
        Destroy(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    private void ShowDeathRpc()
    {
        Instantiate(deathEffect, gameManager.ScreenToWorldSpace(effectTarget.transform.position), Quaternion.identity);
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        Color textColor = targetWordText.color;
        // Refresh based on the speical word mods
        if (textColor == Color.magenta)
        {
            // Reverse the word
            char[] array = newWord.ToString().ToCharArray();
            Array.Reverse(array);
            targetWordText.text = new string(array);
        }
        else if (textColor == Color.red && targetWordText.text.Contains("???"))
        {
            // In this case no need to refresh the ?s
            return;
        }
        else
        {
            // The generic refresh
            targetWordText.text = newWord.ToString();
        }
    }

    protected override void OnTargetIDChanged(ulong oldID, ulong newID)
    {
        gameManager.RefreshProjectiles();
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
            transform.Translate(direction * (Math.Max(wordSpeed * (float)Math.Pow(modMultiplier, mod), wordSpeedMin)) * Time.deltaTime);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateTextEveryoneRpc(FixedString128Bytes newWord, int special = 0) {
        word = newWord.ToString();
        GetComponent<TMP_Text>().text = word;

        // Special modes
        if (special == 1)
        {
            // Reverse the string, set color to purple
            char[] array = word.ToCharArray();
            Array.Reverse(array);
            word = new string(array);
            GetComponent<TMP_Text>().text = word;
            GetComponent<TMP_Text>().color = Color.magenta;
        }
        else if (special == 2)
        {
            // Mark the string as question marks, set color to red
            word = new string('?', word.Length);
            GetComponent<TMP_Text>().text = word;
            GetComponent<TMP_Text>().color = Color.red;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void UpdateTextClientRpc(FixedString128Bytes newWord, int special = 0, RpcParams rpcParams = default)
    {
        word = newWord.ToString();
        GetComponent<TMP_Text>().text = word;

        // Special modes
        if (special == 2)
        {
            // Set color to red for the specific target
            GetComponent<TMP_Text>().color = Color.red;
        }

    }

    public void SetSpawner(TargetableController obj) { spawner = obj; }
    public void SetTarget(TargetableController obj) { target = obj; }
    public void SetDamage(int damage) { this.damage = damage; }

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
