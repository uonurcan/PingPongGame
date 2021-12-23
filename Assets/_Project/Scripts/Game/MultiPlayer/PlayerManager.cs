using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public enum PlayerNo { PlayerOne, PlayerTwo}
public class PlayerManager : NetworkBehaviour
{
    [SyncVar] public PlayerNo PlayerNo;
    [SyncVar] public int UserID;
    [SyncVar] public float PlayerRacketPower;
    [SyncVar (hook = nameof(SetPlayerScore))] public int PlayerScore;
    [SyncVar] public bool CanMove;
    [SyncVar] public bool HasBall;

    [SerializeField] private Transform ballHolder;
    public Transform BallHolder { get { return ballHolder; } }
    [SerializeField] List<Racket> rackets = new List<Racket>();

    private UserSession m_userSession;
    private GameSceneUIManager m_uIManager;
    private Ball m_ball;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer)
            return;

        Init();

        NetworkIdentity _identity = NetworkClient.connection.identity;
        Racket _userRacket = rackets.Where(r => r.RacketID == m_userSession.UserCurrentRocket).First();
        PlayerRacketPower = _userRacket.RacketPower;

        CmdSetUserID(m_userSession.UserID);
        CmdSetUserRacketPower(PlayerRacketPower);

        if (_identity.isClientOnly) // The player who joined the host
        {
            CmdSetPlayerNo(PlayerNo.PlayerTwo);

            CmdStartGame();
            RotateView();
        }
        else // The player who is host
            CmdSetPlayerNo(PlayerNo.PlayerOne);

        SetPlayerRacket();
        SetPlayerInfo();
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (HasBall && Input.GetMouseButtonDown(0))
            CmdShootBall();
    }

    [ClientCallback]
    private void RotateView()
    {
        Transform _cameraTransform = Camera.main.transform;
        SpriteRenderer _table = GameObject.Find("Table").GetComponent<SpriteRenderer>();

        _cameraTransform.eulerAngles = new Vector3(0f, 0f, 180f);
        _table.flipY = true;
    }

    [ClientCallback]
    private void Init()
    {
        m_userSession = GameObject.FindObjectOfType<UserSession>();
        m_uIManager = GameObject.FindObjectOfType<GameSceneUIManager>();
    }

    [Command]
    private void CmdShootBall()
    {
        HasBall = false;
        m_ball = GameObject.FindObjectOfType<Ball>();
        m_ball.ShootBall(Vector2.up * (transform.position.y > 0 ? -8f : 8f));
    }
    
    [Command]
    private void CmdStartGame()
    {
        MatchManager m_matchManager = GameObject.FindObjectOfType<MatchManager>();
        m_matchManager.SetupAndStartMatch();
    }

    [Command]
    private void CmdSetPlayerNo(PlayerNo playerNo)
    {
        this.PlayerNo = playerNo;
    }

    [Command]
    private void CmdSetUserID(int userID)
    {
        this.UserID = userID;
    }

    [Command]
    private void CmdSetUserRacketPower(float racketPower)
    {
        this.PlayerRacketPower = racketPower;
    }

    [ClientCallback]
    public async void SetPlayerInfo()
    {
        string _username = await UsersRepository.GetUsernameByID(m_userSession.UserID);
        m_uIManager.AssignUsername(_username);
        m_uIManager.AssignUserPoints(0);
    }

    [ClientCallback]
    public async void SetPlayerRacket()
    {
        string _racketId = await UserInventory.GetEquippedRacket(m_userSession.UserID);

        SpriteRenderer _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = rackets.Where(r => r.RacketID == _racketId).First().RacketImage;
    }

    [ClientCallback]
    public void SetPlayerScore(int oldScore, int newScore)
    {
        if (isLocalPlayer)
        {
            m_uIManager.AssignUserPoints(newScore);
        }
    }

    [TargetRpc]
    public async void SetOpponentInfo(int opponentUserID)
    {
        if (!isLocalPlayer) return;

        string _username = await UsersRepository.GetUsernameByID(opponentUserID);

        m_uIManager.AssignOpponentName(_username);
        m_uIManager.AssignOpponentPoints(0);
    }

    [TargetRpc]
    public async void SetOpponentRacket(int opponentUserID, GameObject opponentPlayer)
    {
        if (!isLocalPlayer) return;

        string _racketId = await UserInventory.GetEquippedRacket(opponentUserID);

        SpriteRenderer _spriteRenderer = opponentPlayer.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = rackets.Where(r => r.RacketID == _racketId).First().RacketImage;
    }

    [TargetRpc]
    public void SetOpponentScore(int score)
    {
        if (!isLocalPlayer) return;

        m_uIManager.AssignOpponentPoints(score);
    }

    [TargetRpc]
    public void PlayerLost()
    {
        if (!isLocalPlayer) return;

        MenuHandler _menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        _menuHandler.ToggleMenu(1);
    }

    [TargetRpc]
    public async void PlayerWon()
    {
        if (!isLocalPlayer) return;

        MenuHandler _menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        bool done = await UsersRepository.UpdateUserPoints(11, UserID);

        if (done)
            _menuHandler.ToggleMenu(0);
    }
}
