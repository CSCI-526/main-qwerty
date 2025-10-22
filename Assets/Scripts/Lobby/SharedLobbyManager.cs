using TMPro;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedLobbyManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_Text lobbyCodeText;

    [SerializeField] private string gameScene = "MainScene";

    private async void Start()
    {
        UpdatePlayerCountText();
        lobbyCodeText.text = NetworkManager.Singleton.GetComponent<ConnectionManager>().GetLobbyCode();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdatePlayerCountText();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdatePlayerCountText();
    }

    public void UpdatePlayerCountText()
    {
        if (NetworkManager.Singleton != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            playerCountText.text = $"Players: {playerCount}/4";
        }
    }

    public void StartGame()
    {
        // Load scene over distributed authority session
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
