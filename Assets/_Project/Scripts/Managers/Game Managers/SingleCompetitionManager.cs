using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SingleCompetitionManager : MonoBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * SingleCompetitionManager:
      * This script will manage the Scene_Game_Single.
      */

    private GameSceneUIManager m_gameSceneUIManager; // Scene_Game_Single UI-Manager.
    private UserSession m_activeSession;
    private MenuHandler m_menuHandler;
    private BallBehaviour m_ballBehaviour; // Game Ball.

    [Header("Stats")]
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private bool gameEnded = false;
    [SerializeField] private int maxPoints;
    [SerializeField] private int userPoints = 0;
    [SerializeField] private int opponentPoints = 0;
    [SerializeField] private int starterPlayerNo = 0;

    [Header("Settings")]
    [SerializeField] private List<GameObject> gameRackets;
    [SerializeField] private SpriteRenderer competitionTableSprite;
    [SerializeField] private float tableDistanceFromEdges;
    [SerializeField] private Transform[] sideDetectors;

    [Header("Data")]
    [SerializeField] private List<GameSetting> gameSettings;
    [SerializeField] private List<Racket> racketItems;
    [SerializeField] private List<Table> tableItems;

    private static GameSetting m_currentSetting;
    private static Vector2 m_tableBounds;
    private static float m_maxRacketX;
    private static float m_aIAccuracy;

    public static GameSetting CurrentSetting { get { return m_currentSetting; } }
    public static float MaxRacketX { get { return m_maxRacketX; } }
    public static float AIAccuracy { get { return m_aIAccuracy; } }
    public bool GameStarted { get { return gameStarted; }}
    public bool GameEnded { get { return gameEnded; } }


    private void Awake()
    {
        Init();
        SetupGame();
    }

    private void Start()
    {
        if (Time.timeScale != 0)
            StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        m_ballBehaviour.ShowBall();
        yield return new WaitForSeconds(1f);
        m_ballBehaviour.RespawnBall();
        yield return new WaitForSeconds(0.2f);
        gameStarted = true;
    }
    
    public void UserScored()
    {
        AddPoint("User");
    }

    public void AIScored()
    {
        AddPoint("AI");
    }

    // Every time a player scores, turn (player who should start next round) will change.
    public void ChangeTurn()
    {
        if (gameStarted)
        {
            if (starterPlayerNo == 0) starterPlayerNo = 1;
            else if (starterPlayerNo == 1) starterPlayerNo = 0;
        }

        AssignBall();
    }

    // Give the ball to the starter player.
    private void AssignBall()
    {
        gameRackets[starterPlayerNo].GetComponent<IRocket>().GrabBall();
    }

    // Add points if a player scored.
    private async void AddPoint(string playerName)
    {
        if (playerName == "User")
            userPoints++;
        else
            opponentPoints++;

        // Update UI
        m_gameSceneUIManager.AssignUserPoints(userPoints);
        m_gameSceneUIManager.AssignOpponentPoints(opponentPoints);
        // ....

        if (m_currentSetting.MaxScoreToWin == 0) return; // for practice mode.

        // Player Won.
        if (userPoints == m_currentSetting.MaxScoreToWin)
        {
            gameStarted = false;
            gameEnded = true;

            if (m_activeSession.UserID == 1)
            {
                m_menuHandler.ToggleMenu(2);
                return;
            }

            int _totalPoints = await UsersRepository.GetUserPoints(m_activeSession.UserID);
            int _newPoints = _totalPoints + userPoints;

            // Add all points to player database.
            bool done = await UsersRepository.UpdateUserPoints(_newPoints, m_activeSession.UserID);

            if (done)
                m_menuHandler.ToggleMenu(2); // Open 'player won' menu.
            else
                print("Operation failed!");
        }

        // Player Lost.
        if (opponentPoints == m_currentSetting.MaxScoreToWin)
        {
            gameStarted = false;
            gameEnded = true;
            m_menuHandler.ToggleMenu(1); // Open 'player lost' menu.
        }
    }

    // Setup overall game settings and UI.
    private async void SetupGame()
    {
        ResizeTabel();

        // First get the game mode that the player has chooses.
        GameType _currentGameType = m_activeSession.GameType;
        switch (_currentGameType)
        {
            case GameType.singleEasy:
                m_currentSetting = gameSettings[0];
                break;
            case GameType.singleModerate:
                m_currentSetting = gameSettings[1];
                break;
            case GameType.singleHard:
                m_currentSetting = gameSettings[2];
                break;
            case GameType.practice:
                m_currentSetting = gameSettings[3];
                break;
        }
        // ....

        maxPoints = m_currentSetting.MaxScoreToWin; // Set game max point to win.
        m_aIAccuracy = m_currentSetting.AIRocketAccuracy; // Set AI accuracy.

        // Get player racket.
        Racket _playerRacket = racketItems.Where(r => r.RacketID == m_activeSession.UserCurrentRocket).First();
        // Set player racket in 'PlayerRacketController' to use 'RacketPower' field.
        PlayerRacketController.PlayerRacket = _playerRacket;

        // Set racket and table image of the game based on player equipment.
        gameRackets[0].GetComponent<SpriteRenderer>().sprite = _playerRacket.RacketImage;
        competitionTableSprite.sprite = tableItems.Where(t => t.TableID == m_activeSession.UserCurrentTable).First().TableImage;
        // ....

        // Set username of the player in UI.
        if (m_activeSession.UserID == 1) // If playing as a guest.
        {
            m_gameSceneUIManager.AssignUsername("Guest");
            if (_currentGameType != GameType.practice)
            {
                m_menuHandler.ToggleMenu(3); // Open 'register to save points' menu.
                SetGameTimeScale(0);
            }
        }
        else
            m_gameSceneUIManager.AssignUsername(await UsersRepository.GetUsernameByID(m_activeSession.UserID));
        // ....

        // Set name of the opponent in UI.
        if (m_activeSession.GameType == GameType.practice)
            m_gameSceneUIManager.AssignOpponentName("Practice");
        else
            m_gameSceneUIManager.AssignOpponentName("AI");
        // ....

        // All have 0 points at first.
        m_gameSceneUIManager.AssignUserPoints(0);
        m_gameSceneUIManager.AssignOpponentPoints(0);
        // ....
    }

    // For freeze or unfreeze game in some situations.
    public void SetGameTimeScale(int scale)
    {
        Time.timeScale = scale;
    }

    // Resize game table based on user device screen size, then set max x distance that player can move.
    private void ResizeTabel()
    {
        // Set screen bounds.
        Vector3 _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        SetSideDetectors(_screenBounds.x);

        // Get table bounds.
        m_tableBounds = new Vector2((competitionTableSprite.bounds.size.x / 2), (competitionTableSprite.bounds.size.y / 2));
        // Set max x distance that player can move.
        m_maxRacketX = _screenBounds.x - tableDistanceFromEdges - 0.5f;

        // Resize table.
        float _newScale = (_screenBounds.x - tableDistanceFromEdges) * competitionTableSprite.transform.localScale.x;
        _newScale = _newScale / m_tableBounds.x;

        competitionTableSprite.transform.localScale = new Vector3(_newScale, 0.4f, 1f);
        // ....
    }

    // Set aside collider position based on screen bounds.
    private void SetSideDetectors(float screenX)
    {
        foreach(Transform side in sideDetectors)
        {
            Vector2 _sidePos = side.position;
            float _newX = _sidePos.x;

            if (_sidePos.x > 0)
                _newX = screenX;
            else if (_sidePos.x < 0)
                _newX = -screenX;

            _sidePos.x = _newX;
            side.position = _sidePos;
        }
    }

    // Get bounds of the game (a little more than table bounds).
    // For checking, if the ball is out of scope, so AI don't follow it any more.
    public static Vector2 GetGameBounds()
    {
        Vector2 _bounds = new Vector2();
        _bounds.x = m_tableBounds.x + 0.5f;
        _bounds.y = m_tableBounds.y + 0.5f;
        return _bounds;
    }

    // If guest players decide to register before playing the game.
    public void ClearGuestPrefs()
    {
        PlayerPrefs.DeleteKey("Username");
    }

    // Initialize references.
    private void Init()
    {
        m_gameSceneUIManager = GameObject.FindObjectOfType<GameSceneUIManager>();
        m_activeSession = GameObject.FindObjectOfType<UserSession>();
        m_menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        m_ballBehaviour = GameObject.FindObjectOfType<BallBehaviour>();
    }
}
