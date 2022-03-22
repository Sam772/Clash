using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class ProfileScreen : MenuScreen {

    public TMP_Text statsHeader;

    public void GetPlayerProfile(string playFabId) {
        PlayFabClientAPI.GetPlayerProfile( new GetPlayerProfileRequest() {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints() {
                ShowDisplayName = true
            }
        },
        result => statsHeader.text = result.PlayerProfile.DisplayName + "'s Stats",
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}