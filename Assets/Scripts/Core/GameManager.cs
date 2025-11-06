using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Header("Controller Index Lists")]
    [SerializeField] private NetworkList<PlayerNetworkData> playerData = new NetworkList<PlayerNetworkData>();
    [SerializeField] private NetworkList<EnemyNetworkData> enemyData = new NetworkList<EnemyNetworkData>();
    [SerializeField] private NetworkList<ProjectileNetworkData> projectileData = new NetworkList<ProjectileNetworkData>();
    public PlayerController localPlayer;

    [Header("Controllers")]
    [SerializeField] private Dictionary<ulong, PlayerController> players = new Dictionary<ulong, PlayerController>();
    [SerializeField] private Dictionary<ulong, EnemyController> enemies = new Dictionary<ulong, EnemyController>();
    [SerializeField] private Dictionary<ulong, ProjectileController> projectiles = new Dictionary<ulong, ProjectileController>();

    [Header("GameObjects")]
    [SerializeField] private GameObject projectileParent;
    [SerializeField] private GameObject startBattleButton;
    [SerializeField] private GameObject cursePanel;
    [SerializeField] private GameObject curseMsgPrefab;

    [DoNotSerialize]
    public TypingEffectManager typingEffectManager => FindFirstObjectByType<TypingEffectManager>();
    public NetworkManager networkManager => NetworkManager.Singleton;
    public SharedCanvasController sharedCanvas => FindFirstObjectByType<SharedCanvasController>();
    public GameLoopManager gameLoopManager => FindFirstObjectByType<GameLoopManager>();
    public AnalyticsManager analyticsManager => FindFirstObjectByType<AnalyticsManager>();

    public ulong projectileTargetingIdCounter = 0;

    #region Unity Methods

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        foreach (PlayerNetworkData data in playerData)
        {
            OnPlayerDataChanged(new NetworkListEvent<PlayerNetworkData>
            {
                Type = NetworkListEvent<PlayerNetworkData>.EventType.Add,
                Value = data
            });
        }

        foreach (EnemyNetworkData data in enemyData)
        {
            OnEnemyDataChanged(new NetworkListEvent<EnemyNetworkData>
            {
                Type = NetworkListEvent<EnemyNetworkData>.EventType.Add,
                Value = data
            });
        }

        foreach (ProjectileNetworkData data in projectileData)
        {
            OnProjectileDataChanged(new NetworkListEvent<ProjectileNetworkData>
            {
                Type = NetworkListEvent<ProjectileNetworkData>.EventType.Add,
                Value = data
            });
        }
    }

    private void OnEnable()
    {
        playerData.OnListChanged += OnPlayerDataChanged;
        enemyData.OnListChanged += OnEnemyDataChanged;
        projectileData.OnListChanged += OnProjectileDataChanged;
    }

    private void OnDisable()
    {
        playerData.OnListChanged -= OnPlayerDataChanged;
        enemyData.OnListChanged -= OnEnemyDataChanged;
        projectileData.OnListChanged -= OnProjectileDataChanged;
    }

    #endregion

    #region Players

    private void OnPlayerDataChanged(NetworkListEvent<PlayerNetworkData> changeEvent)
    {
        List<PlayerController> playerObjects = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
        switch (changeEvent.Type)
        {
            case NetworkListEvent<PlayerNetworkData>.EventType.Add:
            case NetworkListEvent<PlayerNetworkData>.EventType.Value:
                PlayerNetworkData addedData = changeEvent.Value;
                if (!players.ContainsKey(addedData.TargetingID))
                {
                    PlayerController player = playerObjects.FirstOrDefault(p => p.targetingID.Value == addedData.TargetingID);
                    if (player != null)
                    {
                        players.Add(addedData.TargetingID, player);
                        if(player.GetPlayerID() == NetworkManager.Singleton.LocalClientId)
                        {
                            localPlayer = player;
                        }
                    }
                }
                break;
            case NetworkListEvent<PlayerNetworkData>.EventType.Remove:
                PlayerNetworkData removedData = changeEvent.Value;
                players.Remove(removedData.TargetingID);
                break;
            case NetworkListEvent<PlayerNetworkData>.EventType.Clear:
                foreach (PlayerController player in playerObjects)
                {
                    if (player != null && player.gameObject != null)
                    {
                        if (player.IsOwner)
                        {
                            player.gameObject.GetComponent<NetworkObject>().Despawn(false);
                            Destroy(player.gameObject);
                        }
                    }
                }
                sharedCanvas.playerPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
                players.Clear();
                break;
        }
    }

    public void SpawnPlayer(ulong requesterClientId, string playerName)
    {
        sharedCanvas.RequestSpawnPlayerIconOwnerRpc(requesterClientId, new FixedString128Bytes(playerName));
    }

    public void AddPlayer(PlayerNetworkData data)
    {
        playerData.Add(data);
    }

    public void RemovePlayer(ulong targetingID)
    {
        foreach (PlayerNetworkData player in playerData)
        {
            if (player.TargetingID == targetingID)
            {
                playerData.Remove(player);
                break;
            }
        }
    }

    public void RemoveAllPlayers()
    {
        playerData.Clear();
    }

    public PlayerController GetPlayerByClientId(ulong clientId)
    {
        return players.TryGetValue(clientId, out PlayerController player) ? player : null;
    }

    public PlayerController GetRandomPlayer()
    {
        if (players.Count == 0) return null;
        List<PlayerController> playerList = players.Values.ToList();
        int randomIndex = Random.Range(0, playerList.Count);
        return playerList[randomIndex];
    }

    public bool IsPlayersDead()
    {
        return players.Count == 0;
    }

    public bool PlayersSpawned()
    {
        return players.Count > 0;
    }

    #endregion

    #region Enemies

    private void OnEnemyDataChanged(NetworkListEvent<EnemyNetworkData> changeEvent)
    {
        List<EnemyController> enemyObjects = FindObjectsByType<EnemyController>(FindObjectsSortMode.None).ToList();
        switch (changeEvent.Type)
        {
            case NetworkListEvent<EnemyNetworkData>.EventType.Add:
            case NetworkListEvent<EnemyNetworkData>.EventType.Value:
                EnemyNetworkData addedData = changeEvent.Value;
                if (!enemies.ContainsKey(addedData.TargetingID))
                {
                    EnemyController enemy = enemyObjects.FirstOrDefault(e => e.targetingID.Value == addedData.TargetingID);
                    if (enemy != null)
                    {
                        enemies.Add(addedData.TargetingID, enemy);
                    }
                }
                break;
            case NetworkListEvent<EnemyNetworkData>.EventType.Remove:
                EnemyNetworkData removedData = changeEvent.Value;
                enemies.Remove(removedData.TargetingID);
                break;
            case NetworkListEvent<EnemyNetworkData>.EventType.Clear:
                foreach (EnemyController enemy in enemyObjects)
                {
                    if (enemy != null && enemy.gameObject != null)
                    {
                        if (enemy.IsOwner)
                        {
                            enemy.gameObject.GetComponent<NetworkObject>().Despawn(false);
                            Destroy(enemy.gameObject);
                        }
                    }
                }
                sharedCanvas.enemyPanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
                enemies.Clear();
                break;
        }
    }

    public void SpawnEnemy()
    {
        sharedCanvas.RequestSpawnEnemyIconOwnerRpc(gameLoopManager.GetEnemyHealthMultiplier(), gameLoopManager.GetEnemyAttackCooldownMultiplier());
    }

    public void AddEnemy(EnemyNetworkData data)
    {
        enemyData.Add(data);
    }

    public void RemoveEnemy(ulong targetingID)
    {
        foreach (EnemyNetworkData enemy in enemyData)
        {
            if (enemy.TargetingID == targetingID)
            {
                enemyData.Remove(enemy);
                break;
            }
        }
    }

    public void RemoveAllEnemies()
    {
        enemyData.Clear();
    }

    public bool IsEnemiesDead()
    {
        return enemies.Count == 0;
    }

    #endregion

    #region Projectiles

    private void OnProjectileDataChanged(NetworkListEvent<ProjectileNetworkData> changeEvent)
    {
        List<ProjectileController> projectileObjects = FindObjectsByType<ProjectileController>(FindObjectsSortMode.None).ToList();
        switch (changeEvent.Type)
        {
            case NetworkListEvent<ProjectileNetworkData>.EventType.Add:
            case NetworkListEvent<ProjectileNetworkData>.EventType.Value:
                ProjectileNetworkData addedData = changeEvent.Value;
                if (!projectiles.ContainsKey(addedData.TargetingID))
                {
                    ProjectileController projectile = projectileObjects.FirstOrDefault(p => p.targetingID.Value == addedData.TargetingID);
                    if (projectile != null)
                    {
                        projectiles.Add(addedData.TargetingID, projectile);
                    }
                }
                break;
            case NetworkListEvent<ProjectileNetworkData>.EventType.Remove:
                ProjectileNetworkData removedData = changeEvent.Value;
                projectiles.Remove(removedData.TargetingID);
                break;
            case NetworkListEvent<ProjectileNetworkData>.EventType.Clear:
                foreach (ProjectileController projectile in projectileObjects)
                {
                    if (projectile != null && projectile.gameObject != null)
                    {
                        if (projectile.IsOwner)
                        {
                            projectile.gameObject.GetComponent<NetworkObject>().Despawn(false);
                            Destroy(projectile.gameObject);
                        }
                    }
                }
                projectiles.Clear();
                break;
        }
    }

    public void AddProjectile(ProjectileNetworkData data)
    {
        projectileData.Add(data);
    }

    public void RemoveProjectile(ulong targetingID)
    {
        foreach (ProjectileNetworkData projectile in projectileData)
        {
            if (projectile.TargetingID == targetingID)
            {
                projectileData.Remove(projectile);
                break;
            }
        }
    }

    public void RemoveAllProjectiles()
    {
        projectileData.Clear();
    }

    public GameObject GetProjectileParent() { return projectileParent; }

    #endregion

    #region Targeting

    public TargetableController GetTargetFromWord(string word)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.Value.IsDead()) continue;
            if (enemy.Value.GetTargetWord().Equals(word))
            {
                return enemy.Value;
            }
        }
        foreach (var player in players)
        {
            if (player.Value.IsDead()) continue;
            if (player.Value.GetTargetWord().Equals(word))
            {
                return player.Value;
            }
        }
        foreach (var projectile in projectiles)
        {
            if (projectile.Value.IsDead()) continue;
            if (projectile.Value.GetTargetWord().Equals(word))
            {
                return projectile.Value;
            }
        }
        return null;
    }

    #endregion

    #region Typing Effect

    [Rpc(SendTo.SpecifiedInParams)]
    public void AddRandomCurseBuffEffectRpc(RpcParams rpcParams)
    {
        const int NUM_CHOICES = 3;
        const int NUM_BUFF = 5;
        const int NUM_CURSE = 6;

        List<Vector2Int> allViablePairs = new();
        for (int x1 = 0; x1 < NUM_BUFF; ++x1)
        {
            for (int x2 = 0; x2 < NUM_CURSE; ++x2)
            {
                if (x1 != x2)
                {
                    allViablePairs.Add(new Vector2Int(x1, x2));
                }
            }
        }

        List<Vector2Int> selectedPairs;
        if (NUM_CHOICES <= allViablePairs.Count)
        {
            // random.sample (non-repeating, without replacement)
            for (int i = 0; i < NUM_CHOICES; ++i)
            {
                int j = Random.Range(i, allViablePairs.Count);
                (allViablePairs[i], allViablePairs[j]) = (allViablePairs[j], allViablePairs[i]);
            }
            selectedPairs = allViablePairs.GetRange(0, NUM_CHOICES);
        }
        else
        {
            // random.choices (possible repeating, with replacement)
            selectedPairs = new List<Vector2Int>();
            for (int i = 0; i < NUM_CHOICES; ++i)
            {
                int j = Random.Range(0, allViablePairs.Count);
                selectedPairs.Add(allViablePairs[j]);
            }
        }

        foreach (var pair in selectedPairs)
        {
            TypingEffectBase randomBuffData = pair.x switch
            {
                0 => typingEffectManager.PunishmentMod(-1),
                1 => typingEffectManager.HealMod(-1),
                2 => typingEffectManager.DamageMod(-1),
                3 => typingEffectManager.BulletSpeedMod(-1),
                4 => typingEffectManager.AllLowercase(),
                // 5 => localPlayer.ModifyCurrentHealth(50),
                _ => null,
            };
            TypingEffectBase randomCurseData = pair.y switch
            {
                0 => typingEffectManager.PunishmentMod(1),
                1 => typingEffectManager.HealMod(1),
                2 => typingEffectManager.DamageMod(1),
                3 => typingEffectManager.BulletSpeedMod(1),
                4 => typingEffectManager.ForceCapitalize((char)Random.Range(65, 91)),
                5 => typingEffectManager.ForceDoubling((char)Random.Range(65, 91)), // TODO: maybe vowels only
                _ => null,
            };

            GameObject go = Instantiate(curseMsgPrefab);
            go.transform.SetParent(cursePanel.transform);

            string newCurseText = "";
            if (randomBuffData != null)
            {
                newCurseText += "New Buff:\n" + randomBuffData.GetEffectDescription() + "\n";
            }
            if (randomCurseData != null)
            {
                newCurseText += "New Curse:\n" + randomCurseData.GetEffectDescription() + "\n";
            }
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newCurseText;

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnBuffCurseSelect(randomBuffData, randomCurseData));
        }

        CustomLayoutGroup layoutGroup = cursePanel.GetComponent<CustomLayoutGroup>();
        layoutGroup.RefreshLayout();

    }
    private void OnBuffCurseSelect(TypingEffectBase buff, TypingEffectBase curse)
    {
        typingEffectManager.AddTypingEffect(buff);
        typingEffectManager.AddTypingEffect(curse);
        cursePanel.SetActive(false);
        if(IsOwner)
            startBattleButton.SetActive(true);

        Transform parent = cursePanel.GetComponent<RectTransform>();
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        cursePanel.GetComponent<CustomLayoutGroup>().RefreshLayout();
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
