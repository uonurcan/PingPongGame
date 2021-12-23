using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkDiscovery m_networkDiscovery;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject waitingForPlayerMenu;
    [SerializeField] private GameObject waitingForGameMenu;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button stopHostButton;
    [SerializeField] private Button stopJoinButton;

    private void Start()
    {
        if (NetworkManager.singleton == null)
            return;

        m_networkDiscovery = GameObject.FindObjectOfType<NetworkDiscovery>();

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            hostButton.onClick.AddListener(StartHost);
            joinButton.onClick.AddListener(JoinHost);
        }

        stopHostButton.onClick.AddListener(StopHost);
        stopJoinButton.onClick.AddListener(StopClient);

        m_networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    private void Update()
    {
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
        }

        if(NetworkClient.isConnected)
            waitingForGameMenu.SetActive(false);

        if (NetworkManager.singleton.numPlayers == 2)
        {
            waitingForPlayerMenu.SetActive(false);
        }

        if(discoveredServers.Count > 0 && !NetworkClient.isConnected)
        {
            foreach (ServerResponse info in discoveredServers.Values)
                    Connect(info);

            discoveredServers.Clear();
        }
    }

    public void StopNetwork()
    {
        if (!NetworkClient.active) return;
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        if (networkIdentity.isClientOnly)
        {
            NetworkManager.singleton.StopClient();
            ShowMultiplayerMenu();
        }
        else
        {
            NetworkManager.singleton.StopHost();
        }
    }

    public void ShowMultiplayerMenu()
    {
        multiplayerMenu.SetActive(true);
    }

    private void StartHost()
    {
        if (!NetworkClient.active)
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            m_networkDiscovery.AdvertiseServer(); multiplayerMenu.SetActive(false);
            waitingForPlayerMenu.SetActive(true);
        }
    }

    private void JoinHost()
    {
        if (!NetworkClient.active)
        {
            discoveredServers.Clear();
            m_networkDiscovery.StartDiscovery();

            multiplayerMenu.SetActive(false);
            waitingForGameMenu.SetActive(true);
        }
    }

    void Connect(ServerResponse info)
    {
        m_networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        discoveredServers[info.serverId] = info;
    }

    private void StopHost()
    {
        NetworkManager.singleton.StopHost();
        multiplayerMenu.SetActive(true);
        waitingForPlayerMenu.SetActive(false);
    }

    private void StopClient()
    {
        m_networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StopClient();
        multiplayerMenu.SetActive(true);
        waitingForGameMenu.SetActive(false);
    }
}
