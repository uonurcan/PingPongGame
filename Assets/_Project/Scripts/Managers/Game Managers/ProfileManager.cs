using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProfileManager : MonoBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * ProfileManager:
      * This script will manage the Scene_Profile.
      */

    [SerializeField] private Sprite[] rackets; // All in-game rackets.
    [SerializeField] private Sprite[] tables; // All in-game tables.
    [SerializeField] private Sprite[] playerRackets; // Rackets that player has.
    [SerializeField] private Sprite[] playerTables; // Tables that player has.

    private ProfileSceneUIManager m_profileSceneUIManager; // Scene_Profile UI-Manager.
    private MenuHandler m_menuHandler;
    private UserSession m_activeSession;

    private int m_racketListIndex;
    private int m_tableListIndex;

    private void Awake()
    {
        Init();
        GetAndAssignUserPoints();
        GetAndSetupUserInventory();
    }

    // Hover user rackets in profile scene.
    public void RacketListHover(int operation)
    {
        m_racketListIndex += operation;
        m_racketListIndex = Mathf.Clamp(m_racketListIndex, 0, playerRackets.Length - 1);

        m_profileSceneUIManager.AssignRacketImage(playerRackets[m_racketListIndex]);
    }

    // Hover user tables in profile scene.
    public void TableListHover(int operation)
    {
        m_tableListIndex =+ operation;
        m_tableListIndex = Mathf.Clamp(m_tableListIndex, 0, playerTables.Length - 1);

        m_profileSceneUIManager.AssignTableImage(playerTables[m_tableListIndex]);
    }

    // Called when user clicks a racket equip button.
    public void EquipRacket()
    {
        EquipRacketRequest();
    }

    public async void EquipRacketRequest()
    {
        bool _done = await UserInventory.ChangeEquippedRacket(m_racketListIndex, m_activeSession);

        if (_done)
            m_menuHandler.ToggleMenu(1); // Open 'racket equipped successfully' menu.
        else
            print("Operation failed!");
    }

    // Called when user clicks a table equip button.
    public void EquipTable()
    {
        EquipTableRequest();
    }

    public async void EquipTableRequest()
    {
        bool _done = await UserInventory.ChangeEquippedTable(m_tableListIndex, m_activeSession);

        if (_done)
            m_menuHandler.ToggleMenu(2); // Open 'table equipped successfully' menu.
        else
            print("Operation failed!");
    }

    // Get user points from the database and assign them in profile scene UI.
    private async void GetAndAssignUserPoints()
    {
        // If the user playing as a guest, assign default value.
        if (m_activeSession.UserID == 1)
        {
            m_profileSceneUIManager.AssignPointsText(0);
            return;
        }
        // ....

        int _points = 0;
        _points = await UsersRepository.GetUserPoints(m_activeSession.UserID);
        m_profileSceneUIManager.AssignPointsText(_points);
    }

    private async void GetAndSetupUserInventory()
    {
        // If the user playing as a guest, set up and assign default values.
        if(m_activeSession.UserID == 1)
        {
            m_profileSceneUIManager.AssignRacketImage(rackets[0]);
            m_profileSceneUIManager.AssignTableImage(tables[0]);

            playerRackets = new Sprite[1];
            playerTables = new Sprite[1];
            playerRackets[0] = rackets[0];
            playerTables[0] = tables[0];

            return;
        }
        // ....

        // Get user rackets from database and assign them in 'playerRackets' list.
        List<string> _userRackets = await UserInventory.GetRacketsList(m_activeSession);
        _userRackets = _userRackets.OrderBy(i => i[1].ToString()).ToList();
        int _currentRacketIndex = int.Parse(m_activeSession.UserCurrentRocket[1].ToString()) - 1;
        m_racketListIndex = _currentRacketIndex;

        m_profileSceneUIManager.AssignRacketImage(rackets[_currentRacketIndex]);

        playerRackets = new Sprite[_userRackets.Count];
        for (int i = 0; i < playerRackets.Length; i++)
        {
            playerRackets[i] = rackets[int.Parse(_userRackets[i][1].ToString()) - 1];
        }
        // ....

        // Get user tables from database and assign them in 'playerTables' list.
        List<string> _userTables = await UserInventory.GetTablesList(m_activeSession);
        _userTables = _userTables.OrderBy(i => i[1].ToString()).ToList();
        int _currentTableIndex = int.Parse(m_activeSession.UserCurrentTable[1].ToString()) - 1;
        m_tableListIndex = _currentTableIndex;

        m_profileSceneUIManager.AssignTableImage(tables[_currentTableIndex]);

        playerTables = new Sprite[_userTables.Count];
        for (int i = 0; i < playerTables.Length; i++)
        {
            playerTables[i] = tables[int.Parse(_userTables[i][1].ToString()) - 1];
        }
        // ....
    }

    // Initialize references.
    private void Init()
    {
        m_profileSceneUIManager = GameObject.FindObjectOfType<ProfileSceneUIManager>();
        m_menuHandler = GameObject.FindObjectOfType<MenuHandler>();
        m_activeSession = GameObject.FindObjectOfType<UserSession>();
    }
}
