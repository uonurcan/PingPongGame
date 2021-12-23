using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * MainMenuManager:
      * This script will manage the main menu part of Scene_Main.
      */

    private MainSceneUIManager m_mainSceneUIManager; // Scene_Main UI-Manager.
    private MenuHandler m_menuHandler;
    private SceneHandler m_sceneHandler;

    private void Awake()
    {
        Init();
    }

    // Called if the script enabled (after logging in).
    private void OnEnable()
    {
        GetAndAssignUsername();
    }

    // The username will be retrieved from the database and will be set in the main menu UI.
    private async void GetAndAssignUsername()
    {
        UserSession _activeSession = GameObject.FindObjectOfType<UserSession>(); // Get active session.
        if (!_activeSession) return;
        if(_activeSession.UserID == 1) // So the user is playing as a guest and has no info in the database.
        {
            m_mainSceneUIManager.AssignUsernameText("Guest");
            return;
        }

        string _username = await UsersRepository.GetUsernameByID(_activeSession.UserID);
        m_mainSceneUIManager.AssignUsernameText(_username);
    }

    // Will be called when the user clicks a game button,
    // Which sets the game mode in the user session and loads the relative game scene.
    public void GoToGame(int gameType)
    {
        UserSession userSession = GameObject.FindObjectOfType<UserSession>();

        GameType _gameType = (GameType)gameType;
        userSession.SetGameType(_gameType);

        if(_gameType != GameType.multi)
            m_sceneHandler.LoadSceneByName("Scene_Game_Single");
        else
        {
            m_sceneHandler.LoadSceneByName("Scene_Game_Multi");
        }
    }

    public void EnterMultiGame()
    {
        UserSession userSession = GameObject.FindObjectOfType<UserSession>();

        if(userSession.UserID == 1) // If the user playing as a guest.
        {
            m_menuHandler.ToggleMenu(9); // Open 'register warning' menu.
            return;
        }

        GoToGame(3);
    }

    // If guest players decide to register before playing multiplayer game.
    public void ClearGuestPrefs()
    {
        PlayerPrefs.DeleteKey("Username");
    }

    // Initialize references.
    private void Init()
    {
        m_mainSceneUIManager = GameObject.FindObjectOfType<MainSceneUIManager>();
        m_menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        m_sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }
}
