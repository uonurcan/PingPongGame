using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIManager : MonoBehaviour
{
    /**
     * This is a UI-Manager script.
     * UI-Manager scripts get and set the main elements of a scene UI that should be altered in code.
     * UI-Manager scripts contain UI elements field, their properties, and some methods as their setters.
     * MainSceneUIManager:
     * This script will manage the Scene_Main UI.
    */

    [Header("UI Elements")]
    [SerializeField] private InputField loginNameInput;
    [SerializeField] private InputField loginPasswordInput;
    [SerializeField] private InputField registerEmailInput;
    [SerializeField] private InputField registerNameInput;
    [SerializeField] private InputField registerPasswordInput;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    public InputField LoginNameInput { get { return loginNameInput; } }
    public InputField LoginPasswordInput { get { return loginPasswordInput; } }
    public InputField RegisterEmailInput { get { return registerEmailInput; } }
    public InputField RegisterNameInput { get { return registerNameInput; } }
    public InputField RegisterPasswordInput { get { return registerPasswordInput; } }
    public TMP_Text UsernameText { get { return usernameText; } }
    public Button LoginButton { get { return loginButton; } }
    public Button RegisterButton { get { return registerButton; } }

    public void AssignUsernameText(string username) => usernameText.text = username;

    public void ToggleLoginButton() => loginButton.interactable = !loginButton.interactable;

    public void ToggleRegisterButton() => registerButton.interactable = !registerButton.interactable;
}
