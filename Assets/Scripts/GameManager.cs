using System.Collections.Generic;
using System.Linq;
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

    public ulong projectileTargetingIdCounter = 0;

    #region Players

    public void SpawnPlayer(ulong requesterClientId, string playerName)
    {
        sharedCanvas.RequestSpawnPlayerIconOwnerRpc(requesterClientId, new FixedString128Bytes(playerName));
    }

    [Rpc(SendTo.Everyone)]
    public void AddPlayerRpc(ulong targetingID)
    {
        players.Add(FindObjectsByType<PlayerController>(FindObjectsSortMode.None).FirstOrDefault(p => p.targetingId == targetingID));
    }

    [Rpc(SendTo.Everyone)]
    public void RemovePlayerRpc(ulong targetingID)
    {
        int index = players.FindIndex(players => players.targetingId == targetingID);
        players.RemoveAt(index);
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

    [Rpc(SendTo.Everyone)]
    public void AddEnemyRpc(ulong targetingID)
    {
        enemies.Add(FindObjectsByType<EnemyController>(FindObjectsSortMode.None).FirstOrDefault(e => e.targetingId == targetingID));
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveEnemyRpc(ulong targetingID)
    {
        int index = enemies.FindIndex(e => e.targetingId == targetingID);
        enemies.RemoveAt(index);
    }

    #endregion

    #region Projectiles

    [Rpc(SendTo.Everyone)]
    public void AddProjectileRpc(ulong targetingID)
    {
        projectiles.Add(FindObjectsByType<ProjectileController>(FindObjectsSortMode.None).FirstOrDefault(p => p.targetingId == targetingID));
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveProjectileRpc(ulong targetingID)
    {
        int index = projectiles.FindIndex(p => p.targetingId == targetingID);
        projectiles.RemoveAt(index);
    }

    public GameObject GetProjectileParent() { return projectileParent; }

    #endregion

    #region Targeting

    public TargetableController GetTargetFromWord(string word)
    {
        Debug.Log("Searching for target with word: " + word);

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

    [Rpc(SendTo.Owner)]
    public void SyncListsRpc(ulong requesterClientId)
    {
        foreach (var player in players)
        {
            SyncPlayersRpc(player.targetingId, RpcTarget.Single(requesterClientId, RpcTargetUse.Temp));
        }
        foreach (var enemy in enemies)
        {
            SyncEnemiesRpc(enemy.targetingId, RpcTarget.Single(requesterClientId, RpcTargetUse.Temp));
        }
        foreach (var projectile in projectiles)
        {
            SyncProjectilesRpc(projectile.targetingId, RpcTarget.Single(requesterClientId, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SyncPlayersRpc(ulong targetingID, RpcParams clientId)
    {
        players.Add(FindObjectsByType<PlayerController>(FindObjectsSortMode.None).FirstOrDefault(p => p.networkedTargetingId.Value == targetingID));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SyncEnemiesRpc(ulong targetingID, RpcParams clientId)
    {
        enemies.Add(FindObjectsByType<EnemyController>(FindObjectsSortMode.None).FirstOrDefault(e => e.networkedTargetingId.Value == targetingID));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SyncProjectilesRpc(ulong targetingID, RpcParams clientId)
    {
        projectiles.Add(FindObjectsByType<ProjectileController>(FindObjectsSortMode.None).FirstOrDefault(p => p.networkedTargetingId.Value == targetingID));
    }

    #endregion
}
