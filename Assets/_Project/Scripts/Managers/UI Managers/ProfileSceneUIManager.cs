using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileSceneUIManager : MonoBehaviour
{
    /**
     * This is a UI-Manager script.
     * UI-Manager scripts get and set the main elements of a scene UI that should be altered in code.
     * UI-Manager scripts contain UI elements field, their properties, and some methods as their setters.
     * ProfileSceneUIManager:
     * This script will manage the Scene_Profile UI.
    */

    [Header("UI Elements")]
    [SerializeField] private TMP_Text userPointsText;
    [SerializeField] private Image userRacketImage;
    [SerializeField] private Image userTableImage;

    public TMP_Text UerPointsText { get { return userPointsText; } }
    public Image UserRacketImage { get { return userRacketImage; } }
    public Image UserTableImage { get { return userTableImage; } }

    public void AssignPointsText(int points) => userPointsText.text = points.ToString();

    public void AssignRacketImage(Sprite image) => userRacketImage.sprite = image;

    public void AssignTableImage(Sprite image) => userTableImage.sprite = image;
}
