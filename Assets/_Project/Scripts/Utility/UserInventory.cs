using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class UserInventory
{
    /**
      * Users rackets and tables are stored in the database in a special format like this:
      * (r1-r3@-r2) or (t1-t2@)
      * The '@' mark determines which racket or table the user is currently equipped with.
      * UserInventory help us work with this string format and make things easy for us in other classes.
      * This class heavily relies on the UsersRepository class.
     */

    public static async Task<List<string>> GetRacketsList(UserSession userSession)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userSession.UserID);
        List<string> _userRackets = _userInventory.Rackets.Split('-').ToList();

        int _currentRacketIndex = _userRackets.IndexOf(_userRackets.Where(t => t.EndsWith("@")).First());
        _userRackets[_currentRacketIndex] = _userRackets[_currentRacketIndex].Remove(2, 1);
        _userRackets = _userRackets.OrderBy(r => r[1].ToString()).ToList();
        return _userRackets;
    }

    public static async Task<List<string>> GetTablesList(UserSession userSession)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userSession.UserID);
        List<string> _userTables = _userInventory.Tables.Split('-').ToList();

        int _currentTableIndex = _userTables.IndexOf(_userTables.Where(t => t.EndsWith("@")).First());
        _userTables[_currentTableIndex] = _userTables[_currentTableIndex].Remove(2, 1);

        return _userTables;
    }

    public static async Task<string> GetEquippedRacket(int userID)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userID);
        string[] _userRackets = _userInventory.Rackets.Split('-');
        string _currentRacket = _userRackets.Where(r => r.EndsWith("@")).First();
        int _currentRacketNo = int.Parse(_currentRacket[1].ToString());

        return "r" + _currentRacketNo;
    }

    public static async Task<string> GetEquippedTable(UserSession userSession)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userSession.UserID);
        string[] _userTables = _userInventory.Tables.Split('-');
        string _currentTable = _userTables.Where(r => r.EndsWith("@")).First();
        int _currentTableNo = int.Parse(_currentTable[1].ToString());

        return "t" + _currentTableNo;
    }

    public static async Task<bool> ChangeEquippedRacket(int racketIndex, UserSession userSession)
    {
        List<string> _racketList = await GetRacketsList(userSession);
        string _finalRacketString = "";

        for (int i = 0; i < _racketList.Count; i++)
        {
            if (_racketList[i].EndsWith("@") && _racketList[i] != _racketList[racketIndex])
                _racketList[i] = _racketList[i].Remove(2, 1);
            else if (!_racketList[i].EndsWith("@") && _racketList[i] == _racketList[racketIndex])
                _racketList[i] = _racketList[i].Insert(2, "@");

            _finalRacketString += _racketList[i] + "-";
        }
        _finalRacketString = _finalRacketString.Remove(_finalRacketString.Length - 1, 1);

        bool _changed = await UsersRepository.UpdateUserRackets(_finalRacketString, userSession.UserID);

        if (_changed)
            userSession.InitCurrentInvenory();

        return _changed;
    }

    public static async Task<bool> ChangeEquippedTable(int racketIndex, UserSession userSession)
    {
        List<string> _tableList = await GetTablesList(userSession);
        string _finalTableString = "";

        for (int i = 0; i < _tableList.Count; i++)
        {
            if (_tableList[i].EndsWith("@") && _tableList[i] != _tableList[racketIndex])
                _tableList[i] = _tableList[i].Remove(2, 1);
            else if (!_tableList[i].EndsWith("@") && _tableList[i] == _tableList[racketIndex])
                _tableList[i] = _tableList[i].Insert(2, "@");

            _finalTableString += _tableList[i] + "-";
        }
        _finalTableString = _finalTableString.Remove(_finalTableString.Length - 1, 1);

        bool _changed = await UsersRepository.UpdateUserTables(_finalTableString, userSession.UserID);

        if (_changed)
            userSession.InitCurrentInvenory();

        return _changed;
    }

    public static async Task<bool> AddRacketToInventory(Racket racket, UserSession userSession)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userSession.UserID);
        _userInventory.Rackets += "-" + racket.RacketID;

        return await UsersRepository.UpdateUserRackets(_userInventory.Rackets, userSession.UserID);
    }

    public static async Task<bool> AddTableToInventory(Table table, UserSession userSession)
    {
        UserInventoryModel _userInventory = await UsersRepository.GetUserInventory(userSession.UserID);
        _userInventory.Tables += "-" + table.TableID;

        return await UsersRepository.UpdateUserTables(_userInventory.Tables, userSession.UserID);
    }
}
