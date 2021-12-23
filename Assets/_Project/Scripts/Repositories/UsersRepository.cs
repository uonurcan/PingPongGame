using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UsersRepository : MonoBehaviour
{
    /**
     * For communicating with database.
     * Note that the first field in every request determines which code should Users.php code execute.
     * All methods use same functionality to send request.
     */
     
    public static async Task<bool> InsertUserInfo(UserInfoModel user)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("AddUser", "AddUser");
        _frm.AddField("Email", user.Email);
        _frm.AddField("Username", user.Username);
        _frm.AddField("Password", user.Password);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return false;
            }
            else
                return true;
        }
    }

    public static async Task<bool> UpdateUserRackets(string rackets, int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("UpdateRackets", "UpdateRackets");
        _frm.AddField("UserID", userID);
        _frm.AddField("Rackets", rackets);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return false;
            }
            else
                return true;
        }
    }

    public static async Task<bool> UpdateUserTables(string tables, int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("UpdateTables", "UpdateTables");
        _frm.AddField("UserID", userID);
        _frm.AddField("Tables", tables);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return false;
            }
            else
                return true;
        }
    }

    public static async Task<bool> UpdateUserPoints(int points, int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("UpdatePoints", "UpdatePoints");
        _frm.AddField("UserID", userID);
        _frm.AddField("Points", points);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return false;
            }
            else
                return true;
        }
    }

    public static async Task<UserInfoModel> GetUserByID(int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserInfoByID", "GetUserInfoByID");
        _frm.AddField("UserID", userID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                if (www.downloadHandler.text == "0")
                {
                    return null;
                }

                string[] _rdr = www.downloadHandler.text.Split('&');

                UserInfoModel _userInfo = new UserInfoModel()
                {
                    UserID = int.Parse(_rdr[0]),
                    Email = _rdr[1],
                    Username = _rdr[2],
                    Password = _rdr[3],
                    Points = int.Parse(_rdr[4])
                };

                return _userInfo;
            }
        }
    }

    public static async Task<UserInfoModel> GetUserByName(string userName)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserInfoByName", "GetUserInfoByName");
        _frm.AddField("Username", userName);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                if (www.downloadHandler.text == "0")
                {
                    return null;
                }

                string[] _rdr = www.downloadHandler.text.Split('&');

                UserInfoModel _userInfo = new UserInfoModel()
                {
                    UserID = int.Parse(_rdr[0]),
                    Email = _rdr[1],
                    Username = _rdr[2],
                    Password = _rdr[3],
                    Points = int.Parse(_rdr[4])
                };

                return _userInfo;
            }
        }
    }

    public static async Task<UserInfoModel> GetUserByEmail(string email)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserInfoByEmail", "GetUserInfoByEmail");
        _frm.AddField("Email", email);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                if (www.downloadHandler.text == "0")
                {
                    return null;
                }

                string[] _rdr = www.downloadHandler.text.Split('&');

                UserInfoModel _userInfo = new UserInfoModel()
                {
                    UserID = int.Parse(_rdr[0]),
                    Email = _rdr[1],
                    Username = _rdr[2],
                    Password = _rdr[3],
                    Points = int.Parse(_rdr[4])
                };

                return _userInfo;
            }
        }
    }

    public static async Task<UserInventoryModel> GetUserInventory(int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserInventory", "GetUserInventory");
        _frm.AddField("UserID", userID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                if (www.downloadHandler.text == "0")
                {
                    return null;
                }

                string[] _rdr = www.downloadHandler.text.Split('&');

                UserInventoryModel _userInfo = new UserInventoryModel()
                {
                    Rackets = _rdr[0],
                    Tables = _rdr[1]
                };

                return _userInfo;
            }
        }
    }

    public static async Task<int> GetUserIDByName(string username)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserIDByName", "GetUserIDByName");
        _frm.AddField("Username", username);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return 0;
            }
            else
            {
                int _userID = int.Parse(www.downloadHandler.text);
                return _userID;
            }
        }
    }

    public static async Task<int> GetUserPoints(int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUserPoints", "GetUserPoints");
        _frm.AddField("UserID", userID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return 0;
            }
            else
            {
                int _points = int.Parse(www.downloadHandler.text);
                return _points;
            }
        }
    }

    public static async Task<string> GetUsernameByID(int userID)
    {
        WWWForm _frm = new WWWForm();
        _frm.AddField("GetUsername", "GetUsername");
        _frm.AddField("UserID", userID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://ayrop.com/PingPong/User.php", _frm))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return "";
            }
            else
            {
                string _username = www.downloadHandler.text;
                return _username;
            }
        }
    }
}