using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField ipInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }

    void HostGame()
    {
        string text = ipInput.text;
        if (text == "") text = "127.0.0.1:7777";
        var parts = text.Split(":");
        string ip = parts[0];
        ushort port = ushort.Parse(parts[1]);
        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport)
        {
            var unityTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            unityTransport.ConnectionData.Address = ip;
            unityTransport.ConnectionData.Port = port;
        }

        NetworkManager.Singleton.StartHost();
        HideConnectScreen();
    }

    void JoinGame()
    {
        string text = ipInput.text;
        var parts = text.Split(":");
        string ip = parts[0];
        ushort port = ushort.Parse(parts[1]);
        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport)
        {
            var unityTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            unityTransport.SetConnectionData(ip, port);
        }
        
        NetworkManager.Singleton.StartClient();
        HideConnectScreen();
    }

    void HideConnectScreen()
    {
        gameObject.SetActive(false);
    }
    
    void ShowConnectScreen()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}