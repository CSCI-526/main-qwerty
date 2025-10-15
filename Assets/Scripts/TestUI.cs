using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TestUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private NetworkVariable<int> testVar = new NetworkVariable<int>();

    private void Start()
    {
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI component is not assigned in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetTextFieldRpc();
            SetTextFieldSelf();
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            IncrementNetworkVariableRpc();
        }
    }

    void SetTextField()
    {
        text.text = "Received RPC";
    }

    void SetTextFieldSelf()
    {
        text.text = "Sent RPC";
    }

    [Rpc(SendTo.NotMe)]
    void SetTextFieldRpc()
    {
        SetTextField();
    }

    [Rpc(SendTo.NotMe)]
    void IncrementNetworkVariableRpc()
    {
        testVar.Value += 1;
        text.text = "Network Variable: " + testVar.Value;
    }
}
