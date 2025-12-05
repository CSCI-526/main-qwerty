using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        if (targetingID.Value == ulong.MaxValue) return;

        if (!modelShown && typeTracker != null && typeTracker.currentClass != null)
        {
            int classType = -1;

            if (typeTracker.currentClass.className.Equals("Balanced"))
                classType = 0;
            else if (typeTracker.currentClass.className.Equals("DPS"))
                classType = 1;
            else if (typeTracker.currentClass.className.Equals("Enchanter"))
                classType = 2;
            else if (typeTracker.currentClass.className.Equals("Healer"))
                classType = 3;

            if (classType == -1) return;

            ShowPlayerModelRpc(NetworkManager.Singleton.LocalClientId, classType);
            modelShown = true;
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ShowPlayerModelRpc(ulong clientId, int classType)
    {
        if (targetingID.Value != clientId) return;

        if (classType == 0)
            playerModels[0].SetActive(true);
        else if (classType == 1)
            playerModels[1].SetActive(true);
        else if (classType == 2)
            playerModels[2].SetActive(true);
        else if (classType == 3)
            playerModels[3].SetActive(true);
    }

    #region Network Variable Methods
    public void SetPlayerID(ulong id) { playerId.Value = id; }
    public ulong GetPlayerID() { return playerId.Value; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
    #endregion
}
