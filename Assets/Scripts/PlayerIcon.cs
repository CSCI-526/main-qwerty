using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : NetworkBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text nameText;
    private ulong playerId;

    public NetworkVariable<FixedString128Bytes> PlayerName = new NetworkVariable<FixedString128Bytes>(
        new FixedString128Bytes(""),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public NetworkVariable<Color> IconColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        nameText.text = PlayerName.Value.ToString();
        avatarImage.color = IconColor.Value;

        PlayerName.OnValueChanged += (_, newVal) => nameText.text = newVal.ToString();
        IconColor.OnValueChanged += (_, newColor) => avatarImage.color = newColor;
    }

    public void SetPlayerID(ulong id) => playerId = id;
    public ulong GetPlayerID() { return playerId; }
}
