using Mirror;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerPingPong : NetworkManager
{
    public Transform playerOneTransform;
    public Transform playerTwoTransform;
    public Text txt;
    private GameObject m_ball;

    public override void OnStopHost()
    {
        base.OnStopServer();

        ClearUIAndResetControls();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClearUIAndResetControls();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform _spawnTransform = numPlayers == 0 ? playerOneTransform : playerTwoTransform;
        GameObject _player = Instantiate(playerPrefab, _spawnTransform.position, _spawnTransform.rotation);
        NetworkServer.AddPlayerForConnection(conn, _player);

        Debug.Log("Player added to Server!");

        if (numPlayers == 2)
        {
            m_ball = Instantiate(spawnPrefabs.Find(p => p.name == "Ball"));
            NetworkServer.Spawn(m_ball);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (m_ball != null)
            NetworkServer.Destroy(m_ball);

        ClearUIAndResetControls();
        this.StopHost();
        base.OnServerDisconnect(conn);
    }

    private void ClearUIAndResetControls()
    {
        NetworkManagerUI _networkManagerUI = GameObject.FindObjectOfType<NetworkManagerUI>();
        GameSceneUIManager _gameSceneUIManager = GameObject.FindObjectOfType<GameSceneUIManager>();
        Transform _cameraTransform = Camera.main.transform;
        SpriteRenderer _table = GameObject.Find("Table").GetComponent<SpriteRenderer>();

        _cameraTransform.eulerAngles = new Vector3(0f, 0f, 0f);
        _table.flipY = false;

        _networkManagerUI.ShowMultiplayerMenu();
        _gameSceneUIManager.ClearUI();
    }
}
