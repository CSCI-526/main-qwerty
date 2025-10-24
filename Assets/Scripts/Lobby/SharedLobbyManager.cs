using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SharedLobbyManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_Text lobbyCodeText;

    [SerializeField] private GameObject startGameButton;

    [SerializeField] private string gameScene = "MainScene";

    private async void Start()
    {
        lobbyCodeText.text = NetworkManager.Singleton.GetComponent<ConnectionManager>().GetLobbyCode();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnNetworkSpawn()
    {
        UpdatePlayerCountTextRpc();
        if (IsOwner)
        {
            startGameButton.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdatePlayerCountTextRpc();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdatePlayerCountTextRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void UpdatePlayerCountTextRpc()
    {
        if (NetworkManager.Singleton != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            playerCountText.text = $"Players: {playerCount}/4";
        }
    }

    public void StartGame()
    {
        try
        {
            NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
            Debug.Log($"Loading scene '{gameScene}' via distributed authority session.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load scene: {ex.Message}");
        }
    }
}
