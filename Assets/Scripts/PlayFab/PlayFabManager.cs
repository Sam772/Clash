using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class PlayFabManager : MonoBehaviour {
    void Start() {
        Login();
    }

    void Login() {
        var request = new LoginWithCustomIDRequest {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, onSuccess, OnError);
    }

    void onSuccess(LoginResult result) {
        Debug.Log("Successful login/account created!");
    }

    void OnError(PlayFabError error) {
        Debug.Log("Error while logging in/creating account");
        Debug.Log(error.GenerateErrorReport());
    }
}
