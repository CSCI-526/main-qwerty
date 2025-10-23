using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] GameObject createLobbyUI;
    [SerializeField] GameObject joinLobbyUI;
    [SerializeField] GameObject playerNameUI;
    [SerializeField] GameObject sharedLobbyUI;

    [Header("Lobby UI Elements")]
    [SerializeField] TextMeshProUGUI lobbyCodeText;
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Toggle allowAnalyticsToggle;

    [Header("Warning UI Elements")]
    [SerializeField] GameObject playerNameError;
    [SerializeField] GameObject joinCodeError;
    [SerializeField] GameObject playerNameWarning;
    [SerializeField] GameObject allowAnalyticsWarning;

    private NetworkManager networkManager => NetworkManager.Singleton;

    private string playerName = "";
    private string createLobbyCode = "";

    private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateRandomCode(int length)
    {
        char[] codeChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            codeChars[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(codeChars);
    }

    private void Start()
    {
        createLobbyCode = GenerateRandomCode(6);
        lobbyCodeText.text = createLobbyCode;
    }

    public void SetPlayerName()
    {
        if(playerNameInputField != null)
        {
            if (!allowAnalyticsToggle.isOn) return;
            if (playerNameInputField.text.Length >= 3 && playerNameInputField.text.Length <= 10)
            {
                playerName = playerNameInputField.text;
                playerNameUI.SetActive(false);
                playerNameWarning.SetActive(false);
                allowAnalyticsWarning.SetActive(false);
                joinLobbyUI.SetActive(true);
                createLobbyUI.SetActive(true);
                networkManager.GetComponent<AnalyticsManager>().SetPlayerConsent(true);
            }
            else
                StartCoroutine(ShowTemp(playerNameError, 3f));
        }
    }

    public void CreateLobby()
    {
        networkManager.GetComponent<ConnectionManager>().StartLobby(playerName, createLobbyCode);
    }

    public void JoinLobby()
    {
        string joinCode = joinCodeInputField.text.ToUpper();
        if(joinCode.Length == 6)
            networkManager.GetComponent<ConnectionManager>().StartLobby(playerName, joinCode);
        else
            StartCoroutine(ShowTemp(joinCodeError, 3f));
    }

    private void OnGUI()
    {
        if(networkManager.GetComponent<ConnectionManager>().IsConnected())
        {
            gameObject.SetActive(false);
            sharedLobbyUI.SetActive(true);
        }
    }

    IEnumerator ShowTemp(GameObject obj, float delay)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
