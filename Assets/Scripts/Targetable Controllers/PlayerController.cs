using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : TargetableController
{
    [SerializeField] private PlayerIcon playerIcon;
    private ulong playerId;

    public override void OnNetworkSpawn()
    {
        InitHealth();
        InitTargeting();
        RandomizeTargetWord();
    }

    protected override void Die()
    {
        gameManager.RemovePlayerRpc(targetingId);
    }

    protected override void OnTargetWordChanged(FixedString128Bytes oldWord, FixedString128Bytes newWord)
    {
        targetWordText.text = newWord.ToString();
    }

    #region Network Variable Methods
    [Rpc(SendTo.Everyone)]
    public void SetPlayerIDRpc(ulong id) => playerId = id;
    public ulong GetPlayerID() { return playerId; }

    public void SetPlayerName(string name) { playerIcon.SetPlayerName(name); }
    #endregion
}
