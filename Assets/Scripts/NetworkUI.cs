using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [Header("UI References")]
    public InputField ipInput;
    public Button hostButton;
    public Button joinButton;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
    }

    private void StartHost()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", 7777); // Listen on all local addresses
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started on local network.");
    }

    private void StartClient()
    {
        string ipAddress = ipInput.text.Trim();

        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogWarning("Please enter a valid IP address!");
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, 7777);
        NetworkManager.Singleton.StartClient();
        Debug.Log($"Attempting to join host at {ipAddress}:7777");
    }
}
