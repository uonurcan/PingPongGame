using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    /**
     * This is a UI-Manager script.
     * UI-Manager scripts get and set the main elements of a scene UI that should be altered in code.
     * UI-Manager scripts contain UI elements field, their properties, and some methods as their setters.
     * GameSceneUIManager:
     * This script will manage the Scene_Game_Single/Multi UI.
    */

    [Header("UI Elements")]
    [SerializeField] private TMP_Text opponentNameText;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Text opponentPointsText;
    [SerializeField] private Text userPointsText;

    public TMP_Text OpponentNameText { get { return opponentNameText; } }
    public TMP_Text UsernameText { get { return usernameText; } }
    public Text OpponentPointsText { get { return opponentPointsText; } }
    public Text UserPointsText { get { return userPointsText; } }

    public void AssignUserPoints(int points) => userPointsText.text = points.ToString();

    public void AssignOpponentPoints(int points) => opponentPointsText.text = points.ToString();

    public void AssignUsername(string name) => usernameText.text = name;

    public void AssignOpponentName(string name) => opponentNameText.text = name;

    public void ClearUI()
    {
        userPointsText.text = "";
        opponentPointsText.text = "";
        usernameText.text = "";
        opponentNameText.text = "";
    }
}
