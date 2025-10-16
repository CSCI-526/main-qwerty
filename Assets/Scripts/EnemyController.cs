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

    // Temporary Variables
    [SerializeField] private int minLength = 5;
    [SerializeField] private int maxLength = 8;
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
        InitHealth();
    }

    protected override void Die()
    {
        gameManager.RemoveEnemy(this);
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (IsDead()) return;

        if (attackCd <= 0)
        {
            ShootWord(GenerateWord());
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

        gameManager.AddProjectile(pc);

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

    private string GenerateWord()
    {
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        string[] vowels = { "a", "e", "i", "o", "u" };

        string word = "";

        int requestedLength = UnityEngine.Random.Range(minLength, maxLength + 1);

        // Generate the word in consonant / vowel pairs
        while (word.Length < requestedLength)
        {
            if (requestedLength != 1)
            {
                // Add the consonant
                string consonant = consonants[UnityEngine.Random.Range(0, consonants.Length)];

                if (consonant == "q" && word.Length + 3 <= requestedLength) // check +3 because we'd add 3 characters in this case, the "qu" and the vowel.  Change 3 to 2 to allow words that end in "qu"
                {
                    word += "qu";
                }
                else
                {
                    while (consonant == "q")
                    {
                        // Replace an orphaned "q"
                        consonant = consonants[UnityEngine.Random.Range(0, consonants.Length)];
                    }

                    if (word.Length + 1 <= requestedLength)
                    {
                        // Only add a consonant if there's enough room remaining
                        word += consonant;
                    }
                }
            }

            if (word.Length + 1 <= requestedLength)
            {
                // Only add a vowel if there's enough room remaining
                word += vowels[UnityEngine.Random.Range(0, vowels.Length)];
            }
        }

        return word;
    }

}
