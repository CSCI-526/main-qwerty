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
        gameManager.RemovePlayer(targetingID.Value);
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        targetWordText.text = newWord.ToString();
    }

    protected override void OnTargetIDChanged(ulong oldID, ulong newID)
    {
        gameManager.RemovePlayer(oldID);
        gameManager.AddPlayer(new PlayerNetworkData
        {
            TargetingID = targetingID.Value,
            PlayerName = playerIcon.PlayerName.ToString()
        });
    }

    #region Network Variable Methods
    public void SetPlayerID(ulong id) { playerId.Value = id; }
    public ulong GetPlayerID() { return playerId.Value; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
    #endregion
}
