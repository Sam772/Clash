using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class HostScreen : MenuScreen {
    #pragma warning disable 649
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button createLobbyButton;
    #pragma warning restore 649

    private void Awake() {
        createLobbyButton.interactable = false;
    }

    public void PlayerNameInputChanged(string newPlayerName) {
        ResetValidityState();
    }

    public void CreateLobbyButtonClicked() {
        if (!MenuUtil.IsPlayerNameValid(playerNameInput.text)) return;
        MenuUtil.SaveNameToPlayerPrefs(playerNameInput.text);

        var networkManager = FindObjectOfType<NewNetworkManager>();
        networkManager.StartHost();
    }

    public void ResetValidityState() {
        createLobbyButton.interactable = MenuUtil.IsPlayerNameValid(playerNameInput.text);
    }

    protected override void OnShow() {
        base.OnShow();
        ResetValidityState();
    }

    public void GetPlayerProfile(string playFabId) {
        PlayFabClientAPI.GetPlayerProfile( new GetPlayerProfileRequest() {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints() {
                ShowDisplayName = true
            }
        },
        result => playerNameInput.text = result.PlayerProfile.DisplayName,
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}