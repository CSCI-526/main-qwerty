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
        SetPlayerModel(classType.Value);
        classType.OnValueChanged += OnClassTypeChanged;
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

    private NetworkVariable<ulong> classType = new NetworkVariable<ulong>(
    ulong.MaxValue,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
    );

    private void Update()
    {
        if (targetingID.Value != NetworkManager.Singleton.LocalClientId) return;

        if (!modelShown && typeTracker != null && typeTracker.currentClass != null)
        {

            if (typeTracker.currentClass.className.Equals("Balanced"))
                UpdateClassTypeRpc(0);
            else if (typeTracker.currentClass.className.Equals("DPS"))
                UpdateClassTypeRpc(1);
            else if (typeTracker.currentClass.className.Equals("Enchanter"))
                UpdateClassTypeRpc(2);
            else if (typeTracker.currentClass.className.Equals("Healer"))
                UpdateClassTypeRpc(3);

            if (classType.Value == ulong.MaxValue) return;

            ShowPlayerModelRpc(NetworkManager.Singleton.LocalClientId, classType.Value);
            modelShown = true;
        }
    }

    public void OnClassTypeChanged(ulong oldValue, ulong newValue)
    {
        ShowPlayerModelRpc(targetingID.Value, newValue);
    }

    [Rpc(SendTo.Owner)]
    public void UpdateClassTypeRpc(ulong classTypeValue)
    {
        classType.Value = classTypeValue;
    }

    [Rpc(SendTo.Everyone)]
    private void ShowPlayerModelRpc(ulong clientId, ulong classType)
    {
        if (targetingID.Value != clientId) return;

        SetPlayerModel(classType);
    }

    private void SetPlayerModel(ulong classType)
    {
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
