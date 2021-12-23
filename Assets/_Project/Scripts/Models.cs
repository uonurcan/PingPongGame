/**
 * This script contains base classes, interfaces and enums that will be used and will be created in the project.
 */

// For getting and storing all of the user info as an object.
public class UserInfoModel
{
    public int UserID { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Points { get; set; }
}

// For getting and storing user inventory (rackets/tables) as an object.
public class UserInventoryModel
{
    public string Rackets { get; set; }
    public string Tables { get; set; }
}

// This enum will be used to store the game mode that the player entered,
// So the game scene manager can set up things based on it.
public enum GameType {singleEasy, singleModerate, singleHard, multi, practice, none}

public interface IRocket
{
    void Shoot();

    void GrabBall();
}