using UnityEngine;

public class UserSession : MonoBehaviour
{
    /**
     * UserSession is used to store user information.
     * An object of UserSession will be created when logging into the game.
     * This class provides info about userID, user equipped racket and table and current game type.
     * The info of this class will be used in several scenes for getting and setting user info in UI or database.
     */

    private static UserSession m_instance;

    [SerializeField] private int m_userID;
    [SerializeField] private string m_userCurrentRocket;
    [SerializeField] private string m_userCurrentTable;
    [SerializeField] private GameType m_gameType;

    public int UserID { get { return m_userID; }}
    public string UserCurrentRocket { get { return m_userCurrentRocket; } }
    public string UserCurrentTable { get { return m_userCurrentTable; } }
    public GameType GameType { get { return m_gameType; } }

    private void Start()
    {
        // Apply singleton pattern for this class.
        if (m_instance != null)
            Destroy(this);
        else
            m_instance = this;

        DontDestroyOnLoad(m_instance);
        // ....
    }

    public void InitSession(int userID)
    {
        m_userID = userID; // Set userID when logging in.
        InitCurrentInvenory(); // Get user Inventory (racket/table) info from database.
    }

    public async void InitCurrentInvenory()
    {
        // If userID is 1 it means user logged in as a guest and not registered in database,
        // So we set user inventory info manually to default values.
        if(m_userID == 1)
        {
            m_userCurrentRocket = "r1";
            m_userCurrentTable = "t1";
            return;
        }

        m_userCurrentRocket = await UserInventory.GetEquippedRacket(this.UserID); // Get equipped racket from database.
        m_userCurrentTable = await UserInventory.GetEquippedTable(this); // Get equipped table from database.
    }

    // When entering a game, by using this method the game type that is entered will be set.
    public void SetGameType(GameType gameType)
    {
        m_gameType = gameType;
    }
}
