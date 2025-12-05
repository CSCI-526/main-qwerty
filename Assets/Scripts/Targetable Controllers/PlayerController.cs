using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : TargetableController
{
    [SerializeField] private PlayerIcon playerIcon;
    private NetworkVariable<ulong> playerId = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        InitHealth();
        InitTargeting();
        RandomizeTargetWord();
    }

    protected override void Die()
    {
        
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        targetWordText.text = newWord.ToString();
    }

    protected override void OnTargetIDChanged(ulong oldID, ulong newID)
    {
        gameManager.RefreshPlayers();
    }

    [SerializeField] List<GameObject> playerModels;
    TypeTracker typeTracker => FindFirstObjectByType<TypeTracker>();
    bool modelShown = false;

    private void Update()
    {
        if (!modelShown && typeTracker != null && typeTracker.currentClass != null)
        {
            ShowPlayerModel();
        }
    }

    private void ShowPlayerModel()
    {
        if (typeTracker.currentClass.className.Equals("Balanced"))
            playerModels[0].SetActive(true);
        else if (typeTracker.currentClass.className.Equals("DPS"))
            playerModels[1].SetActive(true);
        else if (typeTracker.currentClass.className.Equals("Enchanter"))
            playerModels[2].SetActive(true);
        else if (typeTracker.currentClass.className.Equals("Healer"))
            playerModels[3].SetActive(true);

        modelShown = true;
    }

    #region Network Variable Methods
    public void SetPlayerID(ulong id) { playerId.Value = id; }
    public ulong GetPlayerID() { return playerId.Value; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
    #endregion
}
