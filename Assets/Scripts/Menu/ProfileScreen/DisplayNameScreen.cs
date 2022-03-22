using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class DisplayNameScreen : MenuScreen {
    public TMP_InputField nameInput;
    [SerializeField] private MainMenu mainMenu;

    public void SubmitDisplayName() {
        var request = new UpdateUserTitleDisplayNameRequest {
            DisplayName = nameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    public void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result) {
        Debug.Log("Updated display name");
        mainMenu.ReturnToMainScreenClicked();
    }

    public void OnError(PlayFabError error) {
        Debug.Log(error.GenerateErrorReport());
    }
}
