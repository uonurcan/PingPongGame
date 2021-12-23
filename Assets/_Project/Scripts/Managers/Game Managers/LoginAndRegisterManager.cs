using UnityEngine;

public class LoginAndRegisterManager : MonoBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * LoginAndRegisterManager:
      * This script will manage the login/register part of Scene_Main.
      */

    // Will be instantiated when login/register was successful.
    [SerializeField] private GameObject userSessionPrefab;

    private MainSceneUIManager m_mainSceneUIManager; // Scene_Main UI-Manager.
    private MenuHandler m_menuHandler;
    private MainMenuManager m_mainMenuManager; 

    private void Awake()
    {
        Init();
        CheckIfActiveSession();
    }

    // Check if the user already logged in.
    private void CheckIfActiveSession()
    {
        UserSession _activeSession = GameObject.FindObjectOfType<UserSession>();

        if (_activeSession != null)
        {
            if (PlayerPrefs.HasKey("Username"))
                EnterGame();
            else
                Destroy(_activeSession.gameObject);
        }
        else
        {
            if (PlayerPrefs.HasKey("Username"))
                EnterGame();
        }
    }

    // Login method that will be called when the 'login' button is clicked.
    public void Login()
    {
        LoginUser();
    }

    private async void LoginUser()
    {
        // Get username and password that user entered.
        string _username = m_mainSceneUIManager.LoginNameInput.text;
        string _password = m_mainSceneUIManager.LoginPasswordInput.text;
        // ....
        
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            return;

        // Turn of 'login' button until login operation ends.
        m_mainSceneUIManager.ToggleLoginButton();

        // Create an empty UserInfoModel and check if a user with entered username is available, then compare passwords.
        UserInfoModel _user = null;
        _user = await UsersRepository.GetUserByName(_username);
        bool _loginSuccessful = _user == null ? false : _user.Password == _password;
        // ....

        if (!_loginSuccessful)
        {
            // Show warning
            m_menuHandler.ToggleMenu(4);
        }
        else
        {
            PlayerPrefs.SetString("Username", _user.Username); // Save username.
            EnterGame();
        }

        // Turn on 'login' button.
        m_mainSceneUIManager.ToggleLoginButton();
    }

    // Register method that will be called when the 'register' button is clicked.
    public void Register()
    {
        RegisterUser();
    }

    private async void RegisterUser()
    {
        // Get email, username and password that user entered.
        string _email = m_mainSceneUIManager.RegisterEmailInput.text;
        string _username = m_mainSceneUIManager.RegisterNameInput.text;
        string _password = m_mainSceneUIManager.RegisterPasswordInput.text;
        // ....

        if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            return;

        // Turn of 'register' button until register operation ends.
        m_mainSceneUIManager.ToggleRegisterButton();

        // Check if the email has a valid string format.
        if (!Validator.IsValidEmail(_email))
        {
            m_menuHandler.ToggleMenu(5);
            return;
        }

        // Create an empty UserInfoModel and check if a user with the same info already exists.
        UserInfoModel _user = null;
        _user = await UsersRepository.GetUserByEmail(_email);
        if(_user != null) // A user with the same email exists.
        {
            m_menuHandler.ToggleMenu(6);
            m_mainSceneUIManager.ToggleRegisterButton();
            return;
        }
        _user = await UsersRepository.GetUserByName(_username);
        if(_user != null) // A user with the same username exists.
        {
            m_menuHandler.ToggleMenu(7);
            m_mainSceneUIManager.ToggleRegisterButton();
            return;
        }
        // ....

        // Store the user info.
        _user = new UserInfoModel()
        {
            Email = _email,
            Username = _username,
            Password = _password,
        };
       
        // Inset user info in database.
        bool _done = await UsersRepository.InsertUserInfo(_user);

        if (_done)
        {
            PlayerPrefs.SetString("Username", _user.Username); // Save username.
            m_menuHandler.ToggleMenu(8); // Toggle 'registered successfully' menu.
        }
        else
            print("Something went wrong!");

        m_mainSceneUIManager.ToggleRegisterButton();
    }

    // PlayAsGuest method that will be called when the 'play as a guest' button is clicked.
    public void PlayAsGuest()
    {
        PlayerPrefs.SetString("Username", "Guest");
        EnterGame();
    }

    // Toggle the main menu and create a user session if logging in was successful.
    private async void EnterGame()
    {
        m_menuHandler.ToggleMenu(0); // Close login/register menu.
        m_menuHandler.ToggleMenu(1); // Open main menu.

        // Get userID and create a user session.
        string _username = PlayerPrefs.GetString("Username");
        int _userID = await UsersRepository.GetUserIDByName(_username);
        UserInfoModel _userInfo = await UsersRepository.GetUserByID(_userID);
        CreateUserSession(_userInfo);
        // ....
    }

    private void CreateUserSession(UserInfoModel user)
    {
        UserSession _session = Instantiate(userSessionPrefab).GetComponent<UserSession>();
        _session.InitSession(user.UserID);
        m_mainMenuManager.enabled = true;
    }

    // Initialize references.
    private void Init()
    {
        m_mainSceneUIManager = GameObject.FindObjectOfType<MainSceneUIManager>();
        m_menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        m_mainMenuManager = GameObject.FindObjectOfType<MainMenuManager>();
        m_mainMenuManager.enabled = false;
    }
}
