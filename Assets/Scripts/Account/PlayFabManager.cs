using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabManager : MonoBehaviour {

    [Header("Login Interface")]
    public TMP_Text messageText;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    [SerializeField] private MainMenu mainMenu;

    void Start() {
        HideErrorMessage();
    }

    private void HideErrorMessage() {
        messageText.text = default;
    }

    public void RegisterButton() {
        if (passwordInput.text.Length < 6) {
            messageText.text = "Password is too short!";
            return;
        }
        var request = new RegisterPlayFabUserRequest {
            Email = emailInput.text,
            Password = passwordInput.text,
            // without needing username and email
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        messageText.text = "Registered and logged in!";
        mainMenu.DisplayNameSent();
    }

    public void LoginButton() {
        var request = new LoginWithEmailAddressRequest {
            Email = emailInput.text,
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void OnLoginSuccess(LoginResult result) {
        messageText.text = "Logged in!";
        Debug.Log("Successful login/account created!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (name == null)
            mainMenu.DisplayNameSent();
        else
            mainMenu.ReturnToMainScreenClicked();
    }

    public void ResetPasswordButton() {
        var request = new SendAccountRecoveryEmailRequest {
            Email = emailInput.text,
            TitleId = "37C06"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    public void OnPasswordReset(SendAccountRecoveryEmailResult result) {
        messageText.text = "Password reset email sent!";
    }

    public void OnSuccess(LoginResult result) {
        Debug.Log("Successful login/account created!");
    }

    public void OnError(PlayFabError error) {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
}