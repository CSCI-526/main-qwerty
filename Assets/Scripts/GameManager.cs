using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Controllers")]
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>();
    [SerializeField] private List<ProjectileController> projectiles = new List<ProjectileController>();

    [Header("GameObjects")]
    [SerializeField] private GameObject projectileParent;

    [DoNotSerialize]
    public TypingEffectManager typingEffectManager => FindFirstObjectByType<TypingEffectManager>();
    private SharedCanvasController sharedCanvas => FindFirstObjectByType<SharedCanvasController>();

    #region Players

    public void SpawnPlayer(ulong requesterClientId, string playerName)
    {
        sharedCanvas.RequestSpawnPlayerIconOwnerRpc(requesterClientId, new FixedString128Bytes(playerName));
    }

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

    public void SpawnEnemy()
    {
        sharedCanvas.RequestSpawnEnemyIconOwnerRpc();
    }

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

    #region Targeting

    public TargetableController GetTargetFromWord(string word)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead()) continue;
            if (enemy.GetTargetWord().Equals(word))
            {
                return enemy;
            }
        }
        foreach (var player in players)
        {
            if (player.IsDead()) continue;
            if (player.GetTargetWord().Equals(word))
            {
                return player;
            }
        }
        foreach (var projectile in projectiles)
        {
            if (projectile.IsDead()) continue;
            if (projectile.GetTargetWord().Equals(word))
            {
                return projectile;
            }
        }
        return null;
    }

    #endregion

    #region Misc
    public string GenerateWord()
    {
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        string[] vowels = { "a", "e", "i", "o", "u" };

        string word = "";

        int requestedLength = UnityEngine.Random.Range(5, 8 + 1);

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

    #endregion
}
