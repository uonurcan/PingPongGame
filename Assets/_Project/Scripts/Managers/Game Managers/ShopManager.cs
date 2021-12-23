using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * ShopManager:
      * This script will manage the Scene_Shop.
      */
    
    [SerializeField] private List<Racket> rackets; // All in-game rackets.
    [SerializeField] private List<Table> tables; // All in-game tables.

    private int m_currentRacketItemIndex = 0;
    private int m_currentTableItemIndex = 0;

    private ShopSceneUIManager m_shopSceneUIManager; // Scene_Shop UI-Manager.
    private MenuHandler m_menuHandler;
    private UserSession m_activeSession;

    private void Awake()
    {
        Init();
    }

    // Called when user clicks a racket unlock button.
    public void UnlockRacket()
    {
        UnlockRacketRequest();
    }

    public async void UnlockRacketRequest()
    {
        Racket _racket = rackets[m_currentRacketItemIndex]; // Get selected racket.
        int _userPoints = await UsersRepository.GetUserPoints(m_activeSession.UserID); // Get user points.

        if(_userPoints - _racket.RacketValue >= 0) // Check if user points are enough to buy the racket.
        {
            int _newPoints = _userPoints - _racket.RacketValue; // Subtract the points.

            bool _updatedPoints = await UsersRepository.UpdateUserPoints(_newPoints, m_activeSession.UserID);
            bool _addedRacket = await UserInventory.AddRacketToInventory(_racket, m_activeSession);

            if (_updatedPoints && _addedRacket)
                m_menuHandler.ToggleMenu(2); // Show 'bought successfully' menu.
            else
                print("Operation Failed!");
        }
        else
        {
            m_menuHandler.ToggleMenu(1); // Show 'not enough points' menu.
        }

        SetupShop(); // Update shop.
    }

    // Called when user clicks a table unlock button.
    public void UnlockTable()
    {
        UnlockTableRequest();
    }

    public async void UnlockTableRequest()
    {
        Table _table = tables[m_currentTableItemIndex]; // Get selected table.
        int _userPoints = await UsersRepository.GetUserPoints(m_activeSession.UserID); // Get user points.

        if (_userPoints - _table.TableValue >= 0) // Check if user points are enough to buy the racket.
        {
            int _newPoints = _userPoints - _table.TableValue; // Subtract the points.

            bool _updatedPoints = await UsersRepository.UpdateUserPoints(_newPoints, m_activeSession.UserID);
            bool _addedRacket = await UserInventory.AddTableToInventory(_table, m_activeSession);

            if (_updatedPoints && _addedRacket)
                m_menuHandler.ToggleMenu(3); // Show 'bought successfully' menu.
            else
                print("Operation Failed!");
        }
        else
        {
            m_menuHandler.ToggleMenu(1); // Show 'not enough points' menu.
        }

        SetupShop(); // Update shop.
    }

    // Hover racket items in shop scene.
    public void RacketListHover(int operation)
    {
        m_currentRacketItemIndex += operation;
        m_currentRacketItemIndex = Mathf.Clamp(m_currentRacketItemIndex, 0, rackets.Count - 1);

        Racket _currentRacket = rackets[m_currentRacketItemIndex];

        HandleRacketInfoDisplay(_currentRacket);
    }

    // Hover table items in shop scene.
    public void TableListHover(int operation)
    {
        m_currentTableItemIndex += operation;
        m_currentTableItemIndex = Mathf.Clamp(m_currentTableItemIndex, 0, tables.Count - 1);

        Table _currentTable = tables[m_currentTableItemIndex];

        HandleTableInfoDisplay(_currentTable);
    }

    // Updates shop racket item base on 'racket' info.
    private void HandleRacketInfoDisplay(Racket racket)
    {
        if (racket.Bought)
            m_shopSceneUIManager.SetRacketAsBought();
        else
            m_shopSceneUIManager.DisableRacketBoughtPhase();

        m_shopSceneUIManager.AssignRacketImage(racket.RacketImage);
        m_shopSceneUIManager.SetPowerBarAndText(racket.RacketPower);
        m_shopSceneUIManager.SetRacketValueText(racket.RacketValue);
    }

    // Updates shop racket item base on 'table' info.
    private void HandleTableInfoDisplay(Table table)
    {
        if (table.Bought)
            m_shopSceneUIManager.SetTableAsBought();
        else
            m_shopSceneUIManager.DisableTableBoughtPhase();


        m_shopSceneUIManager.AssignTableImage(table.TableImage);
        m_shopSceneUIManager.SetTableValueText(table.TableValue);
    }

    // Update and assign shop variables and UI.
    private async void SetupShop()
    {
        UserSession _activeSession = GameObject.FindObjectOfType<UserSession>();

        // If the user playing as a guest, assign default values.
        if (_activeSession.UserID == 1)
        {
            rackets[0].SetAsBought();
            tables[0].SetAsBought();

            HandleRacketInfoDisplay(rackets[m_currentRacketItemIndex]);
            HandleTableInfoDisplay(tables[m_currentTableItemIndex]);
            return;
        }
        // ....

        // Get rackets that player already bought from the database.
        List<string> _userRackets = await UserInventory.GetRacketsList(_activeSession);
        // Get tables that player already bought from the database.
        List<string> _userTables = await UserInventory.GetTablesList(_activeSession);

        // Check all available rackets in game, and get them if they are in user inventory
        var _ownedRackets = rackets.Where(r => _userRackets.Contains(r.RacketID));
        // Check all available tables in game, and get them if they are in user inventory
        var _ownedTables = tables.Where(t => _userTables.Contains(t.TableID));

        foreach (var _racket in _ownedRackets)
        {
            _racket.SetAsBought(); // Set 'bought' boolean of owned rackets as true.
        }
        foreach (var _table in _ownedTables)
        {
            _table.SetAsBought(); // Set 'bought' boolean of owned tables as true.
        }

        HandleRacketInfoDisplay(rackets[m_currentRacketItemIndex]);
        HandleTableInfoDisplay(tables[m_currentTableItemIndex]);
    }

    // Initialize references.
    private void Init()
    {
        m_activeSession = GameObject.FindObjectOfType<UserSession>();
        m_shopSceneUIManager = GameObject.FindObjectOfType<ShopSceneUIManager>();
        m_menuHandler = GameObject.FindObjectOfType<MenuHandler>();

        SetupShop();
    }
}
